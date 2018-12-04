using AutoMapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Responces
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
                .ForMember(r => r.Score, map => map.MapFrom(e => e.Score))
                .ForMember(r => r.TaskText, map => map.MapFrom(e => e.ExerciseTask));
            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(e => e.StudentID));
        }
    }
}
