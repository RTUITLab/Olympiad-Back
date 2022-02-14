﻿using DocumentFormat.OpenXml.Office2010.ExcelAc;
using PublicAPI.Responses.ExerciseTestData;
using Refit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using PublicAPI.Requests.Exercises;
using PublicAPI.Responses;
using PublicAPI.Responses.ExercisesTestData;

namespace Olympiad.ControlPanel.Services;

[Headers("Authorization: Bearer")]
public interface IExercisesApi
{

    [Post("/api/exercises")]
    public Task<Guid> CreateExerciseAsync(Guid challengeId);

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
    public Task<List<ExerciseCompactResponse>> GetExercisesWithAttemptsForUserAsync(Guid challengeId, Guid userId);
    
    [Put("/api/exercises/{exerciseId}")]
    Task<ExerciseInfo> UpdateExercise(Guid exerciseId, UpdateExerciseRequest exerciseModel);
    
    [Get("/api/exercises/{exerciseId}/testGroups")]
    Task<List<ExercisesTestDataGroupResponse>> GetTestGroupsAsync(Guid exerciseId);
    
    [Post("/api/exercises/{exerciseId}/testGroups")]
    Task CreateTestGroupsAsync(Guid exerciseId, CreateTestDataGroupRequest createRequest);
    
    [Delete("/api/exercises/{exerciseId}/testGroups/{groupDataId}")]
    Task DeleteTestGroupsAsync(Guid exerciseId, Guid groupDataId);
    
    [Post("/api/exercises/{exerciseId}/recheck")]
    Task<int> RecheckExerciseSolutions(Guid exerciseId);
}
