using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Services.Configure
{
    public interface IConfigureWork
    {
        Task Configure(CancellationToken cancellationToken);
    }
}