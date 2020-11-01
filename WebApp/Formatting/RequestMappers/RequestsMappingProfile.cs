using AutoMapper;
using Models;
using Models.Checking;
using Models.Exercises;
using Models.Lessons;
using PublicAPI.Requests;
using PublicAPI.Requests.Challenges;
using PublicAPI.Requests.Lessons;
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
            CreateMap<ChallengeCreateEditRequest, Challenge>()
                .ForMember(ch => ch.StartTime, map => map.MapFrom(ccr => ccr.StartTime == null ? null as DateTimeOffset? : ccr.StartTime.Value.ToUniversalTime()))
                .ForMember(ch => ch.EndTime, map => map.MapFrom(ccr => ccr.EndTime == null ? null as DateTimeOffset? : ccr.EndTime.Value.ToUniversalTime()));

            CreateMap<SolutionCheckRequest, SolutionCheck>();

            CreateMap<CourseCreateRequest, Course>();
            CreateMap<GroupCreateEditRequest, Group>();
        }
    }
}
