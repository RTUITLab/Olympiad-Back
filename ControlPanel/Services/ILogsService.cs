
using Microsoft.AspNetCore.Components;

namespace Olympiad.ControlPanel.Services;
public interface ILogsService
{
    void Show();
    void LogInfo(string message, string? description = null);
    void LogInfoObject<T>(T message);
    void LogInfoObject<T>(string description, T message);
    RenderFragment<string> OpenLogsButton { get; }
}
