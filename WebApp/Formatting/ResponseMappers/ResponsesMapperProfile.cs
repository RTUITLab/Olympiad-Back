using AutoMapper;
using Models;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses.Users;
using System;
using System.Linq;
using PublicAPI.Responses.Solutions;
using WebApp.Models;
using Models.Checking;
using System.Security.Claims;
using PublicAPI.Responses.Account;
using Models.UserModels;
using PublicAPI.Responses.Challenges.Analytics;
using PublicAPI.Responses.Solutions.Analytics;
using WebApp.Controllers;
using PublicAPI.Responses.ExercisesTestData;
using Olympiad.Shared;
using PublicAPI.Responses.Exercises;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using ByteSizeLib;

namespace WebApp.Formatting.ResponseMappers
{
    public class ResponsesMapperProfile : Profile
    {
        public ResponsesMapperProfile()
        {
            Guid userId = default;
            CreateMap<Exercise, ExerciseCompactInternalModel>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Status, map => map.MapFrom(e => e
                    .Solutions
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.Status)
                    .Select(s => s.Status)
                    .FirstOrDefault()
                    ))
                .ForMember(r => r.ChallengeViewMode, map => map.MapFrom(c => c.Challenge.ViewMode));

            CreateMap<ExerciseCompactInternalModel, ExerciseForUserInfoResponse>()
                .ForMember(ecim => ecim.Status, map => map.MapFrom(ecr => ecr.GetStatus()))
                .ForMember(ecim => ecim.HiddenStatus, map => map.MapFrom(ecr => ecr.GetHiddenStatus()));
            CreateMap<string, ProgramRuntime>().ConvertUsing(ProgramRuntime.FromValue);
            
            CreateMap<ExerciseRestrictions, ExerciseRestrictionsResponse>();
            CreateMap<CodeRestrictions, CodeRestrictionsResponse>();
            
            CreateMap<double, ByteSize>().ConstructUsing(pr => ByteSize.FromBytes(pr));
            CreateMap<DocsRestrictions, DocsRestrictionsResponse>();
            CreateMap<DocumentRestriction, DocumentRestrictionResponse>();
            
            CreateMap<Exercise, ExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.Score)))
                .ForMember(r => r.Restrictions, map => map.Ignore());

            CreateMap<Exercise, AdminExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.Score)))
                .ForMember(r => r.Restrictions, map => map.Ignore());


            CreateMap<Exercise, ExerciseCompactResponse>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.Score)));
            CreateMap<Exercise, AdminExerciseCompactResponse>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.Score)));

           

            CreateMap<Exercise, ExerciseWithTestCasesCountResponse>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.Score)))
                .ForMember(r => r.TestCasesCount, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.ExerciseDatas.Count)));

            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));
            CreateMap<User, UserInfoResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));
            
            CreateMap<SolutionFile, SolutionDocumentResponse>();
            CreateMap<Solution, SolutionInternalModel>()
                .ForMember(sim => sim.ChallengeViewMode, map => map.MapFrom(s => s.Exercise.Challenge.ViewMode));

            CreateMap<SolutionInternalModel, SolutionResponse>()
                .ForMember(sr => sr.Status, map => map.MapFrom(sim => sim.GetStatus()))
                .ForMember(sr => sr.HiddenStatus, map => map.MapFrom(sim => sim.GetHiddenStatus()))
                .ForMember(sr => sr.Documents, map => map.Ignore());
            CreateMap<Solution, SolutionAnalyticCompactResponse>()
                .ForMember(r => r.Score, map => map.MapFrom(s => s.TotalScore))
                .ForMember(r => r.ExerciseType, map => map.MapFrom(s => s.Exercise.Type));
            CreateMap<Solution, SolutionAnalyticsResponse>()
                .ForMember(r => r.Score, map => map.MapFrom(s => s.TotalScore));


            CreateMap<SolutionBuildLog, BuildLogAnalyticsResponse>();

            CreateMap<Challenge, ChallengeResponse>();
            CreateMap<Challenge, ChallengeExtendedResponse>()
                .ForMember(cer => cer.Invited, map => map.MapFrom(c => c.UsersToChallenges.Select(utc => utc.User)));
            CreateMap<Challenge, ChallengeResponseWithAnalytics>()
                .ForMember(a => a.StartedExecutionCount, 
                map => map.MapFrom(c => c
                                    .Exercises
                                    .SelectMany(e => e.Solutions)
                                    .Select(s => s.UserId)
                                    .Distinct()
                                    .Count()))
                .ForMember(a => a.InvitedCount, map => map.MapFrom(c => c.UsersToChallenges.Count()));

            CreateMap<ExerciseDataGroup, ExercisesTestDataGroupResponse>()
                .ForMember(etdgr => etdgr.TestCases, map => map.MapFrom(edg => edg.ExerciseDatas));
            CreateMap<ExerciseData, ExerciseDataResponse>();

            CreateMap<SolutionCheck, SolutionCheckResponse>();

            CreateMap<Claim, ClaimResponseObject>();

            CreateMap<LoginEvent, LoginEventResponse>()
                .ForMember(r => r.Date, map => map.MapFrom(e => e.LoginTime));
        }
    }
}
