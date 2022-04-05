using AutoMapper;
using ByteSizeLib;
using Models;
using Models.Checking;
using Models.Exercises;
using Olympiad.Shared;
using PublicAPI.Requests;
using PublicAPI.Requests.Challenges;
using PublicAPI.Requests.Exercises;

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

            CreateMap<CreateTestCaseRequest, ExerciseData>()
                .ForMember(ed => ed.InData, map => map.MapFrom(cr => cr.In))
                .ForMember(ed => ed.OutData, map => map.MapFrom(cr => cr.Out));
            CreateMap<CreateTestDataGroupRequest, ExerciseDataGroup>()
                .ForMember(edg => edg.ExerciseDatas, map => map.MapFrom(edg => edg.Cases));


            CreateMap<ProgramRuntime, string>().ConstructUsing(pr => pr.Value);
            CreateMap<ByteSize, double>().ConstructUsing(pr => pr.Bytes);
            CreateMap<UpdateCodeRestrictionsRequest, CodeRestrictions>();
            CreateMap<DocumentRestrictionRequest, DocumentRestriction>();
            CreateMap<UpdateDocsRestrictionsRequest, DocsRestrictions>();

            CreateMap<UpdateExerciseRequest, Exercise>();
        }
    }
}
