using Olympiad.Shared.Models;
using PublicAPI.Responses;
using PublicAPI.Responses.ExerciseTestData;
using PublicAPI.Responses.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.HubModels
{
    public interface IHubClient
    {
        Task UpdateSolutionStatus(SolutionResponse solution);
        Task UpdateExerciseStatus(ExerciseForUserInfoResponse exerciseResponse);
        Task InformationMessage(string message);
    }
}
