using AutoMapper;
using Models;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using System.Linq;

namespace WebApp.Formatting.ResponseMappers
{
    public class ResponsesMapperProfile : Profile
    {
        public ResponsesMapperProfile()
        {
            CreateMap<Exercise, ExerciseCompactResponse>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName));
            CreateMap<Exercise, ExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Solutions, map => map.MapFrom(e => e.Solution))
                .ForMember(r => r.TaskText, map => map.MapFrom(e => e.ExerciseTask));
            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));
            CreateMap<User, UserResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));

            CreateMap<Solution, SolutionInfo>()
                .ForMember(si => si.SendingTime, map => map.MapFrom(s => s.Time));

            CreateMap<Challenge, ChallengeResponse>();
            CreateMap<Challenge, ChallengeExtendedResponse>()
                .ForMember(cer => cer.Invited, map => map.MapFrom(c => c.UsersToChallenges.Select(utc => utc.User)));
        }
    }
}
