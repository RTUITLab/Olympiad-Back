using AutoMapper;
using Models;
using Models.Checking;
using Models.Exercises;
using PublicAPI.Requests;

namespace WebApp.ViewModels.Mappings
{
    public class RequestsMappingProfile : Profile
    {
        public RequestsMappingProfile()
        {
            CreateMap<RegistrationRequest, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));

            CreateMap<SolutionCheckRequest, SolutionCheck>();

            CreateMap<UpdateAccountInfoRequest, User>();
        }
    }
}
