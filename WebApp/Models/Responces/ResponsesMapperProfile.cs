﻿using AutoMapper;
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
            CreateMap<Exercise, ExerciseResponce>()
                .ForMember(R => R.Id, map => map.MapFrom(E => E.ExerciseID))
                .ForMember(R => R.Name, map => map.MapFrom(E => E.ExerciseName))
                .ForMember(R => R.Score, map => map.MapFrom(E => E.Score));
        }
    }
}