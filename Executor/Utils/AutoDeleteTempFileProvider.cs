using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Executor.Utils;

public class AutoDeleteTempFileProviderOptions
{
    [Required]
    public string LocalTempFilesRootFolder { get; set; }
    [Required]
    public string HostTempFilesRootFolder { get; set; }
}

internal class AutoDeleteTempFileProvider
{
    private readonly AutoDeleteTempFileProviderOptions _options;

    public AutoDeleteTempFileProvider(IOptions<AutoDeleteTempFileProviderOptions> options)
    {
        if (!Directory.Exists(options.Value.LocalTempFilesRootFolder))
        {
            Directory.CreateDirectory(options.Value.LocalTempFilesRootFolder);
        }
        _options = options.Value;
    }
    public IAutoDeleteTempFile GetTempFile() => new AutoDeleteTempFile(_options, Guid.NewGuid().ToString());

    private class AutoDeleteTempFile(AutoDeleteTempFileProviderOptions options, string fileName) : IAutoDeleteTempFile
    {
        public string LocalFilePath { get; } = Path.Combine(new DirectoryInfo(options.LocalTempFilesRootFolder).FullName, fileName);
        public string HostFilePath { get; } = Path.Combine(new DirectoryInfo(options.HostTempFilesRootFolder).FullName, fileName);

        public void Dispose()
        {
            try
            {
                File.Delete(LocalFilePath);
            }
            catch { }
        }
    }
}
internal interface IAutoDeleteTempFile : IDisposable
{
    /// <summary>
    /// Путь к файлу для записи
    /// </summary>
    string LocalFilePath { get; }
    /// <summary>
    /// Путь к файлу в хост системе. При запуске внутри контейнера для bind нужно
    /// передавать именно путь от хост системы при старте контейнера
    /// </summary>
    string HostFilePath { get; }
}