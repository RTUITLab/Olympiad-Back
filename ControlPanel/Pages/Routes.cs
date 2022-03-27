using System;

namespace Olympiad.ControlPanel.Pages;

public static class Routes
{
    public static class Challenges
    {
        public static string ChallengeEdit(Guid challengeId) => $"challenges/{challengeId}";
    }
}
