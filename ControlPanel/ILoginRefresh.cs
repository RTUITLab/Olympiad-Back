
namespace Olympiad.ControlPanel;
public interface ILoginRefresh
{
    Task SaveTokenAndRefreshAsync(string? accessToken);
}
