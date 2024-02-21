using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Solutions;
using Olympiad.Services.SolutionCheckQueue;
using Olympiad.Shared;
using Olympiad.Shared.Models;
using PublicAPI.Requests.Solutions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services.Attachments;

namespace WebApp.Services.Solutions
{
    public class SolutionsService : ISolutionsService
    {
        private readonly ApplicationDbContext context;
        private readonly IQueueChecker queueChecker;
        private readonly IAttachmentsService attachmentsService;
        private readonly ILogger<SolutionsService> logger;
        private static readonly TimeSpan onePerTimePostLimit = TimeSpan.FromMinutes(1);
        public SolutionsService(
            ApplicationDbContext context, 
            IQueueChecker queueChecker, 
            IAttachmentsService attachmentsService,
            ILogger<SolutionsService> logger)
        {
            this.context = context;
            this.queueChecker = queueChecker;
            this.attachmentsService = attachmentsService;
            this.logger = logger;
        }

        public async Task<Solution> PostDocsSolution(
            Guid exerciseId, Guid authorId, 
            SolutionDocumentRequest[] files, 
            ISolutionsService.CodeSolutionChecks solutionChecks)
        {
            var now = DateTimeOffset.UtcNow;
            await HandleGeneralChecks(exerciseId, authorId, solutionChecks, now);

            
            if (solutionChecks.HasFlag(ISolutionsService.CodeSolutionChecks.DocsFilesIsCorrect))
            {
                await CheckFielsCorrect(exerciseId, files);
            }
            var solution = new Solution()
            {
                ExerciseId = exerciseId,
                UserId = authorId,
                Status = SolutionStatus.InQueue,
                SendingTime = now,
                DocumentsResult = new SolutionDocuments
                {
                    Files = files.Select(d => new SolutionFile
                    {
                        Name = d.Name,
                        MimeType = d.MimeType,
                        Size = d.Size.Bytes
                    })
                    .ToList()
                }
            };
            context.Solutions.Add(solution);

            foreach (var file in files)
            {
                // Получение потока нужно делать последовательно, файл за файлом, потому поток из файла обернут в функцию
                await attachmentsService.UploadSolutionDocument(solution.Id, file.MimeType, file.Name, file.ContentFunc());
            }

            await context.SaveChangesAsync();
            return solution;
        }

        private async Task CheckFielsCorrect(Guid exerciseId, SolutionDocumentRequest[] files)
        {
            var targetExercise = await context
                .Exercises
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.ExerciseID == exerciseId);
            if (targetExercise.Restrictions?.Docs?.Documents?.Count != files.Length ||
                targetExercise.Restrictions.Docs.Documents
                    .Zip(files, (expect, fromUser) => expect.MaxSize >= fromUser.Size.Bytes && expect.AllowedExtensions.Contains(Path.GetExtension(fromUser.Name)))
                    .Any(success => !success))
            {
                throw new ISolutionsService.IncorrectFilesException();
            }

        }

        public async Task<Solution> PostCodeSolution(string fileBody, ProgramRuntime runtime, Guid exerciseId, Guid authorId, ISolutionsService.CodeSolutionChecks solutionChecks)
        {
            var now = DateTimeOffset.UtcNow;
            await HandleGeneralChecks(exerciseId, authorId, solutionChecks, now);
            if (solutionChecks.HasFlag(ISolutionsService.CodeSolutionChecks.CodeAlreadySent))
            {
                await CheckAlreadySent(fileBody, exerciseId, authorId);
            }
            if (solutionChecks.HasFlag(ISolutionsService.CodeSolutionChecks.CodeExerciseRuntimeRestrictions))
            {
                await CheckExerciseRuntime(exerciseId, runtime);
            }

            Solution solution = new Solution()
            {
                Raw = fileBody,
                Language = runtime,
                ExerciseId = exerciseId,
                UserId = authorId,
                Status = SolutionStatus.InQueue,
                SendingTime = now
            };

            await context.Solutions.AddAsync(solution);
            await context.SaveChangesAsync();
            logger.LogInformation("Created solution {SolutionId} for exercise {ExerciseId} and user {UserId}", solution.Id, exerciseId, authorId);
            if (await context.TestData.AnyAsync(td => td.ExerciseDataGroup.ExerciseId == exerciseId))
            {
                logger.LogInformation("Put solution {SolutionId} to test queue", solution.Id);
                queueChecker.PutInQueue(solution.Id);
            }
            else
            {
                logger.LogWarning("No exercise data for {SolutionId}, no put to test queue", solution.Id);
            }
            return solution;
        }

        private async Task HandleGeneralChecks(Guid exerciseId, Guid authorId, ISolutionsService.CodeSolutionChecks solutionChecks, DateTimeOffset now)
        {
            if (solutionChecks.HasFlag(ISolutionsService.CodeSolutionChecks.ChallengeAvailable))
            {
                await CheckChallengeAvailable(exerciseId, authorId, now);
            }
            if (solutionChecks.HasFlag(ISolutionsService.CodeSolutionChecks.TooManyPost))
            {
                await CheckTooManyRequests(exerciseId, authorId, now);
            }
        }

        private async Task CheckExerciseRuntime(Guid exerciseId, ProgramRuntime programRuntime)
        {
            var restrictions = await context.Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .Select(ex => ex.Restrictions)
                .SingleAsync();
            var allowedRuntimes = restrictions?.Code?.AllowedRuntimes?.Select(ProgramRuntime.FromValue) ?? ProgramRuntime.List;
            if (!allowedRuntimes.Contains(programRuntime))
            {
                throw new ISolutionsService.ExerciseRuntimesRestrictionException(allowedRuntimes);
            }
        }

        private async Task CheckChallengeAvailable(Guid exerciseId, Guid authorId, DateTimeOffset now)
        {
            if (!await context.Exercises.AnyAsync(
                                e => e.ExerciseID == exerciseId &&
                                (e.Challenge.StartTime == null || e.Challenge.StartTime <= now) &&
                                (e.Challenge.EndTime == null || e.Challenge.EndTime >= now) &&
                                (e.Challenge.ChallengeAccessType == ChallengeAccessType.Public || e.Challenge.UsersToChallenges.Any(utc => utc.UserId == authorId))))
            {
                throw new ISolutionsService.ChallengeNotAvailableException();
            }
        }

        private async Task CheckAlreadySent(string fileBody, Guid exerciseId, Guid authorId)
        {
            var oldSolution = await context
                                .Solutions
                                .Where(s => s.UserId == authorId)
                                .Where(s => s.Raw == fileBody)
                                .Where(s => s.ExerciseId == exerciseId)
                                .FirstOrDefaultAsync();
            if (oldSolution != null)
            {
                throw new ISolutionsService.AlreadySentException();
            }
        }

        private async Task CheckTooManyRequests(Guid exerciseId, Guid authorId, DateTimeOffset now)
        {
            var lastSendingDate = (await context
                .Solutions
                .Where(s => s.UserId == authorId)
                .Where(s => s.ExerciseId == exerciseId)
                .Select(s => s.SendingTime)
                .ToListAsync())
                .DefaultIfEmpty(DateTimeOffset.MinValue)
                .Max();

            if ((now - lastSendingDate) < onePerTimePostLimit)
            {
                throw new ISolutionsService.TooManyPostException(1, onePerTimePostLimit);
            }
        }
    }
}
