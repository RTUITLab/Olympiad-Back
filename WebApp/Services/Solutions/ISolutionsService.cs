using Models.Solutions;
using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Services.Solutions
{
    public interface ISolutionsService
    {
        Task<Solution> PostSolution(string fileBody, ProgramRuntime runtime, Guid exerciseId, Guid authorId, PostSolutionChecks postSolutionChecks = PostSolutionChecks.AlreadySent | PostSolutionChecks.TooManyPost);

        [Flags]
        public enum PostSolutionChecks
        {
            ChallengeAvailable = 0b0001,
            TooManyPost = 0b0010,
            AlreadySent = 0b0100,
            ExerciseRuntimeRestrictions = 0b1000,
        }

        public class NotFoundEntityException : Exception { public NotFoundEntityException(string entityName) : base("Not found entity: " + entityName) { } }
        public class ChallengeNotAvailableException : Exception { public ChallengeNotAvailableException() { } }
        public class TooManyPostException : Exception
        {
            public TooManyPostException(int tryCount, TimeSpan period) : base($"Only {tryCount} in {period} is allowed") { }
        }
        public class AlreadySentException : Exception { public AlreadySentException() : base("That solution already sent") { } }
        public class ExerciseRuntimesRestrictionException : Exception
        {
            public IEnumerable<ProgramRuntime> AllowerRuntimes { get; }
            public ExerciseRuntimesRestrictionException(IEnumerable<ProgramRuntime> allowedRuntimes)
            {
                AllowerRuntimes = allowedRuntimes;
            }
        }
    }
}
