
namespace Olympiad.ControlPanel.Services;
public interface ILogsService
{
    void Show();
    void LogInfo(string message, string? description = null);
}
