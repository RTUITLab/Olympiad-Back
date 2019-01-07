using AutoMapper;
using Models;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Responses;

namespace WebApp.Formatting.ResponseMappers
{
    public class ResponsesMapperProfile : Profile
    {
        public ResponsesMapperProfile()
        {
            CreateMap<Exercise, ExerciseListResponse>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Score, map => map.MapFrom(e => e.Score));
            CreateMap<Exercise, ExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Solutions, map => map.MapFrom(e => e.Solution))
                .ForMember(r => r.TaskText, map => map.MapFrom(e => e.ExerciseTask));
            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(e => e.StudentID));
            CreateMap<Solution, SolutionInfo>()
                .ForMember(si => si.SendingTime, map => map.MapFrom(s => s.Time));
        }
    }
}
