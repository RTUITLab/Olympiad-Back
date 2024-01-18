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
        return Directory.GetFiles("./SupportedRuntimes")
            .Select(f => File.ReadAllText(f))
            .Select(f =>
            {
                var lineBreakPosition = f.IndexOf('\n');
                return new SupportedRuntime(f[..lineBreakPosition], f[(lineBreakPosition + 1)..]);
            })
            .ToArray();
    }
}
