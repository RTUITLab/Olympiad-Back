using AutoMapper;
using Models;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses.Users;
using Shared.Models;
using System;
using System.Linq;
using PublicAPI.Responses.Dump;

namespace WebApp.Formatting.ResponseMappers
{
    public class ResponsesMapperProfile : Profile
    {
        public ResponsesMapperProfile()
        {
            Guid userId = default;
            CreateMap<Exercise, ExerciseCompactResponse>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Status, map => map.MapFrom(e => (SolutionStatus)e
                    .Solutions
                    .Where(s => s.UserId == userId)
                    .Select(s => (int)s.Status)
                    .DefaultIfEmpty(-1)
                    .Max()));

            CreateMap<Exercise, ExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Solutions, map => map.MapFrom(e => e.Solutions));
            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));
            CreateMap<User, UserInfoResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));

            CreateMap<Solution, SolutionResponse>();

            CreateMap<Challenge, ChallengeResponse>();
            CreateMap<Challenge, ChallengeExtendedResponse>()
                .ForMember(cer => cer.Invited, map => map.MapFrom(c => c.UsersToChallenges.Select(utc => utc.User)));


            CreateMap<ExerciseData, ExerciseDataResponse>();
            CreateMap<ExerciseData, ExerciseDataCompactResponse>();

            CreateMap<Solution, SolutionDumpView>()
                .ForMember(d => d.ExerciseName, map => map.MapFrom(s => s.Exercise.ExerciseName))
                .ForMember(d => d.UserId, map => map.MapFrom(s => s.User.StudentID))
                .ForMember(d => d.ExerciseScore, map => map.MapFrom(s => s.Exercise.Score));

        }
    }
}
