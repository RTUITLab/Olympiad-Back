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

            CreateMap<ExerciseCompactInternalModel, ExerciseCompactResponse>()
                .ForMember(ecim => ecim.Status, map => map.MapFrom(ecr => ecr.GetStatus()))
                .ForMember(ecim => ecim.HiddenStatus, map => map.MapFrom(ecr => ecr.GetHiddenStatus()));

            CreateMap<Exercise, ExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.ExerciseDataGroups.Sum(g => g.Score)))
                .ForMember(r => r.Solutions, map => map.Ignore());


            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));
            CreateMap<User, UserInfoResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));

            CreateMap<Solution, SolutionInternalModel>()
                .ForMember(sim => sim.ChallengeViewMode, map => map.MapFrom(s => s.Exercise.Challenge.ViewMode));
            CreateMap<SolutionInternalModel, SolutionResponse>()
                .ForMember(sr => sr.Status, map => map.MapFrom(sim => sim.GetStatus()))
                .ForMember(sr => sr.HiddenStatus, map => map.MapFrom(sim => sim.GetHiddenStatus()));

            CreateMap<Challenge, ChallengeResponse>();
            CreateMap<Challenge, ChallengeExtendedResponse>()
                .ForMember(cer => cer.Invited, map => map.MapFrom(c => c.UsersToChallenges.Select(utc => utc.User)));


            CreateMap<ExerciseData, ExerciseDataResponse>();
            CreateMap<ExerciseData, ExerciseDataCompactResponse>();

            CreateMap<SolutionCheck, SolutionCheckResponse>();
        }
    }
}
