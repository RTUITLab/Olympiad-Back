using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Solutions;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using PublicAPI.Responses.ExerciseTestData;
using PublicAPI.Responses.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Extensions;
using WebApp.Hubs;
using WebApp.Models;
using WebApp.Models.HubModels;

namespace WebApp.Services
{
    public class NotifyUsersService
    {
        private readonly IHubContext<SolutionStatusHub, IHubClient> hubContext;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public NotifyUsersService(
            IHubContext<SolutionStatusHub, IHubClient> hubContext, 
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.hubContext = hubContext;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task NewSolutionAdded(Solution solution)
        {
            var solutionInternal = await dbContext
                .Solutions
                .Where(s => s.Id == solution.Id)
                .ProjectTo<SolutionInternalModel>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            await SolutionStatusChanged(mapper.Map<SolutionResponse>(solutionInternal));


            var exerciseInternal = await dbContext
                .Exercises
                .Where(ex => ex.ExerciseID == solution.ExerciseId)
                .ProjectTo<ExerciseCompactInternalModel>(mapper.ConfigurationProvider, new { userId = solution.UserId })
                .SingleOrDefaultAsync();
            var exercise = mapper.Map<ExerciseForUserInfoResponse>(exerciseInternal);
            await ExerciseStatusChanged(solution.UserId, exercise);
        }

        public async Task SendInformationMessageToUser(Guid userId, string message)
        {
            await hubContext.Clients.User(userId)
                .InformationMessage(message);
        }

        private Task SolutionStatusChanged(SolutionResponse solutionResponse)
        {
            return hubContext.Clients.User(solutionResponse.UserId)
                .UpdateSolutionStatus(solutionResponse);
        }
        private Task ExerciseStatusChanged(Guid userId, ExerciseForUserInfoResponse exerciseForUserInfo)
        {
            return hubContext.Clients.User(userId.ToString())
                .UpdateExerciseStatus(exerciseForUserInfo);
        }
    }
}
