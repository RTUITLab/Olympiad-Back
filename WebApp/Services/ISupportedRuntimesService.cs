using PublicAPI.Responses.SupportedRuntimes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services;

public interface ISupportedRuntimesService
{
    public ValueTask<SupportedRuntime[]> GetSupportedRuntimes();
}

public sealed class FromFilesCachedSupportedRuntimesService : ISupportedRuntimesService
{
    private Lazy<SupportedRuntime[]> supportedRuntimes;
    public FromFilesCachedSupportedRuntimesService()
    {
        supportedRuntimes = new Lazy<SupportedRuntime[]>(ReadFromDirectory);
    }
    public ValueTask<SupportedRuntime[]> GetSupportedRuntimes()
        => ValueTask.FromResult(supportedRuntimes.Value);

    private SupportedRuntime[] ReadFromDirectory()
    {
        return [..Directory.GetFiles("./SupportedRuntimes")
            .Select(File.ReadAllText)
            .Select(f =>
            {
                var webKeyEndPosition = f.IndexOf('\n');
                var webKey = f[..webKeyEndPosition];

                var humanTitleEndPosition = f.IndexOf('\n', webKeyEndPosition + 1);
                var humanTitle = f[(webKeyEndPosition + 1)..humanTitleEndPosition];

                var acceptFileNameEndPosition = f.IndexOf('\n', humanTitleEndPosition + 1);
                var acceptFileName = f[(humanTitleEndPosition + 1)..acceptFileNameEndPosition];

                var description = f[(acceptFileNameEndPosition + 1)..];
                return new SupportedRuntime(webKey, humanTitle, acceptFileName, description);
            })
            .OrderBy(r => r.Title)];
    }
}
