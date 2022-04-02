using Models.Solutions;
using Olympiad.Shared;
using PublicAPI.Requests.Solutions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Services.Solutions
{
    public interface ISolutionsService
    {
        Task<Solution> PostCodeSolution(string fileBody, ProgramRuntime runtime, Guid exerciseId, Guid authorId, CodeSolutionChecks postSolutionChecks = CodeSolutionChecks.CodeAlreadySent | CodeSolutionChecks.TooManyPost);
        Task<(Solution solution, string[] uploadUrls)> PostDocsSolution(Guid exerciseId, Guid authorId, List<SolutionDocumentRequest> files, CodeSolutionChecks solutionChecks);

        [Flags]
        public enum CodeSolutionChecks
        {
            None = 0b00000,
            ChallengeAvailable = 0b00001,
            TooManyPost = 0b00010,
            CodeAlreadySent = 0b00100,
            CodeExerciseRuntimeRestrictions = 0b01000,
            DocsFilesIsCorrect = 0b10000,

            All = ~None
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

        public class IncorrectFilesException : Exception { public IncorrectFilesException() { } }
    }
}
