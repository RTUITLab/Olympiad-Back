using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Solutions;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
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
            await SolutionStatusChanged(solution);
            var exerciseInternal = await dbContext
                .Exercises
                .Where(ex => ex.ExerciseID == solution.ExerciseId)
                .ProjectTo<ExerciseCompactInternalModel>(mapper.ConfigurationProvider, new { userId = solution.UserId })
                .SingleOrDefaultAsync();
            var exercise = mapper.Map<ExerciseCompactResponse>(exerciseInternal);
            await ExerciseStatusChanged(solution.UserId, exercise);
        }

        public async Task SendInformationMessageToUser(Guid userId, string message)
        {
            await hubContext.Clients.User(userId)
                .InformationMessage(message);
        }

        private Task SolutionStatusChanged(Solution solution)
        {
            return hubContext.Clients.User(solution.UserId)
                .UpdateSolutionStatus(mapper.Map<SolutionResponse>(mapper.Map<SolutionInternalModel>(solution)));
        }
        private Task ExerciseStatusChanged(Guid userId, ExerciseCompactResponse exerciseCompactResponse)
        {
            return hubContext.Clients.User(userId.ToString())
                .UpdateExerciseStatus(exerciseCompactResponse);
        }
    }
}
