using AutoMapper;
using Models;
using Models.Exercises;
using PublicAPI.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.ViewModels.Mappings
{
    public class RequestsMappingProfile : Profile
    {
        public RequestsMappingProfile()
        {
            CreateMap<RegistrationRequest, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
            CreateMap<ExerciseRequest, Exercise>();
        }
    }
}
