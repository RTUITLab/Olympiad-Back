
using System.Threading.Tasks;

namespace Olympiad.ControlPanel;
public interface ILoginRefresh
{
    Task SaveTokenAndRefreshAsync(string? accessToken);
}
