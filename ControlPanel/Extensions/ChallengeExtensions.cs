using Olympiad.Shared.Models;
using PublicAPI.Responses.Challenges.Analytics;

namespace Olympiad.ControlPanel.Extensions;

public static class ChallengeExtensions
{
    public static string HasAccessCountPretty(this ChallengeResponseWithAnalytics challenge)
        => challenge.ChallengeAccessType switch
        {
            ChallengeAccessType.Public => "Все пользователи",
            ChallengeAccessType.Private => challenge.InvitedCount.ToString(),
            _ => "Неверное значение"
        };
}
