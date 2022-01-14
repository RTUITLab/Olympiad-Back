using DocumentFormat.OpenXml.Office2010.ExcelAc;
using PublicAPI.Responses.Exercises;
using Refit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using PublicAPI.Responses;

namespace Olympiad.ControlPanel.Services;

[Headers("Authorization: Bearer")]
public interface IExercisesApi
{
    [Get("/api/exercises/all")]
    public Task<List<ExerciseCompactResponse>> GetExercisesAsync(Guid challengeId);

    [Get("/api/exercises/all/{exerciseId}")]
    public Task<ExerciseInfo> GetExerciseAsync(Guid exerciseId);

    [Get("/api/exercises/{exerciseId}/attachment")]
    public Task<List<AttachmentResponse>> GetExerciseAttachmentsAsync(Guid exerciseId);
    [Delete("/api/exercises/{exerciseId}/attachment/{fileName}")]
    public Task DeleteExerciseAttachmentAsync(Guid exerciseId, string fileName);

    [Get("/api/exercises/{exerciseId}/attachment/upload/{fileName}")]
    public Task<UploadFileUrlResponse> GetUploadAttachmentUrl(Guid exerciseId, string mimeType, long contentLength, string fileName);

    [Get("/api/exercises/all/withtests")]
    public Task<List<ExerciseWithTestCasesCountResponse>> GetExercisesWithTestsAsync(Guid challengeId);

    [Get("/api/exercises/analytics/withAttempt")]
    public Task<List<ExerciseCompactResponse>> GetExercisesWithAtteptsForUserAsync(Guid challengeId, Guid userId);
}
