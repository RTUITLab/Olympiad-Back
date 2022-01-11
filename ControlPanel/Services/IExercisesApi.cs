using DocumentFormat.OpenXml.Office2010.ExcelAc;
using PublicAPI.Responses.Exercises;
using Refit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Olympiad.ControlPanel.Services
{
    public interface IExercisesApi
    {
        [Get("/api/exercises/all")]
        public Task<List<ExerciseCompactResponse>> GetExercisesAsync(Guid challengeId);
        [Get("/api/exercises/all/withtests")]
        public Task<List<ExerciseWithTestCasesCountResponse>> GetExercisesWithTestsAsync(Guid challengeId);

        [Get("/api/exercises/analytics/withAttempt")]
        public Task<List<ExerciseCompactResponse>> GetExercisesWithAtteptsForUserAsync(Guid challengeId, Guid userId);
    }
}
