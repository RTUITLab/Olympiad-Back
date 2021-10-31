using AutoMapper;
using Models;
using Models.Checking;
using Models.Exercises;
using PublicAPI.Requests;
using PublicAPI.Requests.Challenges;

namespace WebApp.ViewModels.Mappings
{
    public class RequestsMappingProfile : Profile
    {
        public RequestsMappingProfile()
        {
            CreateMap<CreateUserDataModel, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));

            CreateMap<SolutionCheckRequest, SolutionCheck>();

            CreateMap<UpdateAccountInfoRequest, User>();

            CreateMap<UpdateChallengeInfoRequest, Challenge>()
                .ForMember(c => c.ChallengeAccessType, map => map.MapFrom(ucir => ucir.AccessType));
        }
    }
}
