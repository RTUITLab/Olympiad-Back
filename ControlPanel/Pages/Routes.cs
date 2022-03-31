using System;

namespace Olympiad.ControlPanel.Pages;

public static class Routes
{
    public static class Users
    {
        public static string UserEdit(Guid userId) => $"users/{userId}";
    }
    public static class Challenges
    {
        public static string ChallengeEdit(Guid challengeId) => $"challenges/{challengeId}";
    }
    public static class Exercises
    {
        public static string ExerciseEdit(Guid challengeId, Guid exerciseId) => $"challenges/{challengeId}/exercises/{exerciseId}";
    }
}
