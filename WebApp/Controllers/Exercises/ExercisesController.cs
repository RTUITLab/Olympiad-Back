﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Models.Exercises;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using WebApp.Models;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;
using ByteSizeLib;
using Olympiad.Shared;
using System.ComponentModel.DataAnnotations;
using PublicAPI.Requests.Exercises;
using WebApp.Services.Attachments;
using Olympiad.Services;
using Microsoft.Extensions.Logging;
using Olympiad.Services.SolutionCheckQueue;
using PublicAPI.Responses.Exercises;
using Npgsql;

namespace WebApp.Controllers.Exercises;

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
    public async Task<Guid> CreateDefaultExerciseAsync([FromBody] ExerciseCreateRequest createRequest)
    {
        var newExercise = new Exercise
        {
            ExerciseName = "NEW EXERCISE NAME",
            ChallengeId = createRequest.ChallengeId,
            ExerciseTask = "FILL EXERCISE TASK",
            Restrictions = new ExerciseRestrictions(),
            Type = createRequest.Type
        };
        createRequest.Type
            .When(ExerciseType.Code).Then(() => newExercise.Restrictions.Code = new CodeRestrictions
            {
                AllowedRuntimes = ProgramRuntime.List.Select(l => l.Value).ToList()
            })
            .When(ExerciseType.Docs).Then(() => newExercise.Restrictions.Docs = new DocsRestrictions
            {
                Documents = new List<DocumentRestriction>()
            });
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
    public async Task<List<AdminExerciseCompactResponse>> GetAllForChallenge(Guid challengeId)
    {
        var exercises = await context
            .Exercises
            .Where(e => e.ChallengeId == challengeId)
            .OrderBy(e => e.ExerciseName)
            .ProjectTo<AdminExerciseCompactResponse>(mapper.ConfigurationProvider)
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

    [HttpGet]
    [Route("{exerciseId:guid}")]
    public async Task<ExerciseInfo> Get(Guid exerciseId)
    {
        var adminExercise = await GetExercise(exerciseId, AvailableExercise(UserId));
        return mapper.Map<ExerciseInfo>(adminExercise);
    }

    [HttpGet("all/{exerciseId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<AdminExerciseInfo> GetForAdmin(Guid exerciseId)
    {
        return await GetExercise(exerciseId, null);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("{exerciseId:guid}")]
    public async Task<ActionResult> Delete(Guid exerciseId)
    {
        var targetExercise = await context.Exercises.FindAsync(exerciseId);
        if (targetExercise == null)
        {
            return NotFound();
        }
        context.Exercises.Remove(targetExercise);
        await context.SaveChangesAsync();
        return new EmptyResult();
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

    [HttpPost("{exerciseId:guid}/attachment/upload")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UploadFileUrlResponse>> UploadAttachment(
        Guid exerciseId,
        [Required] IFormFile attachment,
        [FromServices] IAttachmentsService attachmentsService)
    {
        if (!await context.Exercises.AnyAsync(e => e.ExerciseID == exerciseId))
        {
            return NotFound("Exercise not found");
        }
        var uploadSize = ByteSize.FromBytes(attachment.Length);
        if (uploadSize > AttachmentLimitations.MaxAttachmentSize)
        {
            return BadRequest($"Maximum attachment size is {AttachmentLimitations.MaxAttachmentSize} (sent {uploadSize})");
        }
        await attachmentsService.UploadExerciseAttachment(exerciseId, attachment.ContentType, attachment.FileName, attachment.OpenReadStream());
        return new UploadFileUrlResponse
        {
            Url = Url.Action(nameof(GetAttachment), nameof(ExercisesController), new { exerciseId, fileName = attachment.FileName }),
        };
    }

    [HttpGet("{exerciseId:guid}/attachment/{fileName}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetAttachment(
        Guid exerciseId,
        string fileName,
        [FromServices] IAttachmentsService attachmentsService)
    {
        var (fileStream, contentType) = await attachmentsService.GetExerciseAttachment(exerciseId, fileName);
        return File(fileStream, contentType, fileName);
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


    private async Task<AdminExerciseInfo> GetExercise(Guid exerciseId, Expression<Func<Exercise, bool>> exerciseFilter)
    {
        var targetExerciseQuery = context
            .Exercises
            .Where(ex => ex.ExerciseID == exerciseId);
        if (exerciseFilter != null)
        {
            targetExerciseQuery = targetExerciseQuery.Where(exerciseFilter);
        }
        var exerciseInfo = await targetExerciseQuery
            .ProjectTo<AdminExerciseInfo>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync()
            ?? throw StatusCodeException.NotFount;
        // TODO: incorrect jsonb mapping, hand made
        var restrictions = await context
            .Exercises
            .Where(ex => ex.ExerciseID == exerciseId)
            .Select(ex => ex.Restrictions)
            .SingleAsync();
        exerciseInfo.Restrictions = mapper.Map<ExerciseRestrictionsResponse>(restrictions);
        if (exerciseInfo.Restrictions is null)
        {
            exerciseInfo.Restrictions = new ExerciseRestrictionsResponse();
            exerciseInfo.Type
                .When(ExerciseType.Code).Then(() => exerciseInfo.Restrictions.Code = new CodeRestrictionsResponse
                {
                    AllowedRuntimes = ProgramRuntime.List.ToList()
                })
                .When(ExerciseType.Docs).Then(() => exerciseInfo.Restrictions.Docs = new DocsRestrictionsResponse
                {
                    Documents = new List<DocumentRestrictionResponse>()
                });
        }
        return exerciseInfo;
    }

    [HttpPut("{exerciseId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminExerciseInfo>> UpdateExerciseBaseInfo(Guid exerciseId, UpdateExerciseRequest request)
    {

        var exercise = await context
                           .Exercises
                           .Where(ex => ex.ExerciseID == exerciseId)
                           .SingleOrDefaultAsync()
                       ?? throw StatusCodeException.NotFount;

        mapper.Map(request, exercise);

        exercise.Restrictions ??= new ExerciseRestrictions();
        await context.SaveChangesAsync();
        return await GetForAdmin(exerciseId);
    }

    [HttpPut("{exerciseId:guid}/transferToChallenge")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExerciseInfo>> TransferToChallenge(Guid exerciseId, Guid targetChallengeId)
    {
        var targetExercise = await context.Exercises.FindAsync(exerciseId);
        if (targetExercise is null)
        {
            return NotFound("exercise not found");
        }
        targetExercise.ChallengeId = targetChallengeId;
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is PostgresException pex && pex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            logger.LogWarning(dbEx, "Can't change challenge to {ChallengeId} for exercise {ExerciseId}", targetChallengeId, exerciseId);
            return NotFound("Challenge not found");
        }
        return await GetForAdmin(exerciseId);
    }

    [HttpPost("{exerciseId:guid}/clone")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExerciseInfo>> CreateClone(Guid exerciseId)
    {
        var targetExercise = await context.Exercises
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.ExerciseID == exerciseId);
        if (targetExercise is null)
        {
            return NotFound("exercise not found");
        }
        targetExercise.ExerciseID = default;
        targetExercise.ExerciseName += " clone";
        targetExercise.ExerciseName = targetExercise.ExerciseName[..Math.Min(ExerciseLimitations.MAX_EXERCISE_TITLE_LENGTH, targetExercise.ExerciseName.Length)];
        context.Exercises.Add(targetExercise);
        await context.SaveChangesAsync();
        return await GetForAdmin(targetExercise.ExerciseID);
    }


    [HttpPut("{exerciseId:guid}/restrictions/code")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CodeRestrictionsResponse>> UpdateExerciseCodeRestrictions(Guid exerciseId, UpdateCodeRestrictionsRequest request)
    {

        var exercise = await context
                           .Exercises
                           .Where(ex => ex.ExerciseID == exerciseId)
                           .SingleOrDefaultAsync();
        if (exercise is null)
        {
            return NotFound("Not found exercise");
        }
        if (exercise.Type != ExerciseType.Code)
        {
            return Conflict("Can't change code restrictions for non code exercise");
        }

        exercise.Restrictions ??= new ExerciseRestrictions();
        exercise.Restrictions.Code = mapper.Map<CodeRestrictions>(request);

        // TODO: can't check property is changed, hand made
        context.Entry(exercise).Property(e => e.Restrictions).IsModified = true;

        await context.SaveChangesAsync();
        return mapper.Map<CodeRestrictionsResponse>(exercise.Restrictions.Code);
    }

    [HttpPut("{exerciseId:guid}/restrictions/docs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DocsRestrictions>> UpdateExerciseDocsRestrictions(Guid exerciseId, UpdateDocsRestrictionsRequest request)
    {

        var exercise = await context
                           .Exercises
                           .Where(ex => ex.ExerciseID == exerciseId)
                           .SingleOrDefaultAsync();
        if (exercise is null)
        {
            return NotFound("Not found exercise");
        }
        if (exercise.Type != ExerciseType.Docs)
        {
            return Conflict("Can't change docs restrictions for non docs exercise");
        }

        exercise.Restrictions ??= new ExerciseRestrictions();
        exercise.Restrictions.Docs = mapper.Map<DocsRestrictions>(request);

        // TODO: can't check property is changed, hand made
        context.Entry(exercise).Property(e => e.Restrictions).IsModified = true;

        await context.SaveChangesAsync();
        return mapper.Map<DocsRestrictions>(exercise.Restrictions.Docs);
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
                    db => db.Solutions
                        .Where(s => s.Exercise.Type == ExerciseType.Code)
                        .Where(s => s.ExerciseId == exerciseId),
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