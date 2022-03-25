using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Models.Exercises;
using Models.Solutions;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using WebApp.Models;
using PublicAPI.Requests;
using AutoMapper.QueryableExtensions;
using PublicAPI.Responses.ExerciseTestData;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using WebApp.Models.Settings;
using WebApp.Services;
using ByteSizeLib;
using Olympiad.Shared;
using System.ComponentModel.DataAnnotations;
using PublicAPI.Requests.Exercises;
using WebApp.Services.Attachments;
using Olympiad.Services;
using Microsoft.Extensions.Logging;
using Olympiad.Services.SolutionCheckQueue;

namespace WebApp.Controllers.Exercises
{
    [Produces("application/json")]
    [Route("api/exercises")]
    [Authorize(Roles = "User")]
    public class ExercisesController : AuthorizeController
    {
        private readonly IMapper mapper;
        private readonly ILogger<ExercisesController> logger;
        private readonly ApplicationDbContext context;

        public ExercisesController(
            ApplicationDbContext applicationDbContext,
            IMapper mapper,
            ILogger<ExercisesController> logger,
            UserManager<User> userManager) : base(userManager)
        {
            context = applicationDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<Guid> CreateDefaultExerciseAsync(Guid challengeId)
        {
            var newExercise = new Exercise
            {
                ExerciseName = "NEW EXERCISE NAME",
                ChallengeId = challengeId,
                ExerciseTask = "FILL EXERCISE TASK"
            };
            context.Exercises.Add(newExercise);
            await context.SaveChangesAsync();
            return newExercise.ExerciseID;
        }

        [HttpGet]
        public async Task<List<ExerciseForUserInfoResponse>> GetForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .Where(AvailableExercise(UserId))
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactInternalModel>(mapper.ConfigurationProvider, new { userId = UserId })
                .ToListAsync();
            return exercises.Select(e => mapper.Map<ExerciseForUserInfoResponse>(e)).ToList();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<List<ExerciseCompactResponse>> GetAllForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return exercises;
        }

        [HttpGet("all/withTests")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<List<ExerciseWithTestCasesCountResponse>> GetAllWithTestsForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseWithTestCasesCountResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return exercises;
        }

        [HttpGet("all/{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ExerciseInfo> GetForAdmin(Guid exerciseId)
        {
            var exercise = await context
                .Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .ProjectTo<ExerciseInfo>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            return exercise;
        }

        [HttpGet("{exerciseId:guid}/attachment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AttachmentResponse>>> GetAttachments(Guid exerciseId,
            [FromServices] IAttachmentsService attachmentsService)
        {
            if (!await context.Exercises.AnyAsync(e => e.ExerciseID == exerciseId))
            {
                return NotFound("Exercise not found");
            }
            return (await attachmentsService.GetAttachmentsForExercise(exerciseId))
                .Select(t => new AttachmentResponse
                {
                    FileName = t.fileName,
                    MimeType = t.contentType
                })
                .ToList();
        }

        [HttpGet("{exerciseId:guid}/attachment/upload/{fileName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UploadFileUrlResponse>> UploadAttachment(
            Guid exerciseId,
            [Required] string fileName,
            [Required] string mimeType,
            [Required] long contentLength,
            [FromServices] IAttachmentsService attachmentsService)
        {
            if (!await context.Exercises.AnyAsync(e => e.ExerciseID == exerciseId))
            {
                return NotFound("Exercise not found");
            }
            var uploadSize = ByteSize.FromBytes(contentLength);
            if (uploadSize > AttachmentLimitations.MaxAttachmentSize)
            {
                return BadRequest($"Maximum attachment size is {AttachmentLimitations.MaxAttachmentSize} (sent {uploadSize})");
            }
            return new UploadFileUrlResponse
            {
                Url = attachmentsService.GetUploadUrlForExercise(exerciseId, mimeType, uploadSize, fileName)
            };
        }

        [HttpGet("{exerciseId:guid}/attachment/{fileName}")]
        [AllowAnonymous]
        public ActionResult GetAttachment(
            Guid exerciseId,
            string fileName,
            [FromServices] IAttachmentsService attachmentsService)
        {
            return Redirect(attachmentsService.GetUrlForExerciseAttachment(exerciseId, fileName));
        }
        [HttpDelete("{exerciseId:guid}/attachment/{fileName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAttachment(
            Guid exerciseId,
            string fileName,
            [FromServices] IAttachmentsService attachmentsService)
        {
            if (!await context.Exercises.AnyAsync(e => e.ExerciseID == exerciseId))
            {
                return NotFound("Exercise not found");
            }
            await attachmentsService.DeleteExerciseAttachment(exerciseId, fileName);
            return NoContent();
        }

        [HttpGet]
        [Route("{exerciseId:guid}")]
        public async Task<ExerciseInfo> Get(Guid exerciseId)
        {
            var exercise = await context
                .Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .Where(AvailableExercise(UserId))
                .ProjectTo<ExerciseInfo>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;

            return exercise;
        }

        [HttpPut("{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ExerciseInfo> UpdateExerciseBaseInfo(Guid exerciseId, UpdateExerciseRequest request)
        {

            var exercise = await context
                               .Exercises
                               .Where(ex => ex.ExerciseID == exerciseId)
                               .SingleOrDefaultAsync()
                           ?? throw StatusCodeException.NotFount;
            exercise.ExerciseName = request.Title;
            exercise.ExerciseTask = request.Task;
            await context.SaveChangesAsync();
            return await GetForAdmin(exerciseId);
        }

        [HttpPost("{exerciseId:guid}/recheck")]
        [Authorize(Roles = "Admin")]
        public async Task<int> RecheckExerciseSolutions(
            [FromServices] IQueueChecker queueChecker,
            [FromRoute] Guid exerciseId)
        {
            var recheckedSolutionsCount = await ReCheckHelper.ReCheckSolutions(
                        context,
                        queueChecker,
                        db => db.Solutions.Where(s => s.ExerciseId == exerciseId),
                        m =>
                        {
                            logger.LogInformation(m);
                            return Task.CompletedTask;
                        });
            return recheckedSolutionsCount;
        }

        private Expression<Func<Exercise, bool>> AvailableExercise(Guid userId)
        {
            return ex =>
                // Can get info only for started challenges
                ex.Challenge.StartTime == null || ex.Challenge.StartTime <= Now &&
                (
                    // Can get exercise for publick challenge
                    ex.Challenge.ChallengeAccessType == ChallengeAccessType.Public ||
                    // Or for private for invite
                    ex.Challenge.UsersToChallenges.Any(utc => utc.UserId == userId)
                );

        }
    }
}