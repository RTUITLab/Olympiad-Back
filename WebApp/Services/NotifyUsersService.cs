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
            var exerciseStatus = await dbContext
                .Exercises
                .Where(ex => ex.ExerciseID == solution.ExerciseId)
                .ProjectTo<ExerciseCompactResponse>(mapper.ConfigurationProvider, new { userId = solution.UserId })
                .SingleOrDefaultAsync();
            await ExerciseStatusChanged(solution.UserId, solution.ExerciseId, exerciseStatus.Status);
        }

        public async Task SendInformationMessageToUser(Guid userId, string message)
        {
            await hubContext.Clients.User(userId)
                .InformationMessage(message);
        }

        private Task SolutionStatusChanged(Solution solution)
        {
            return hubContext.Clients.User(solution.UserId)
                .UpdateSolutionStatus(mapper.Map<SolutionResponse>(solution));
        }
        private Task ExerciseStatusChanged(Guid userId, Guid exerciseId, SolutionStatus exerciseStatus)
        {
            return hubContext.Clients.User(userId.ToString())
                .UpdateExerciseStatus(new UpdateExerciseStatusModel
                {
                    ExerciseId = exerciseId,
                    ExerciseStatus = exerciseStatus
                });
        }
    }
}
