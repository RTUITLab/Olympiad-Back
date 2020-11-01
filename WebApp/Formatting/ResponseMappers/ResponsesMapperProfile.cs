using AutoMapper;
using Models;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses.Users;
using Olympiad.Shared.Models;
using System;
using System.Linq;
using PublicAPI.Responses.Dump;
using PublicAPI.Responses.Solutions;
using WebApp.Models;
using Models.Checking;
using Models.Lessons;
using PublicAPI.Responses.Lessons;

namespace WebApp.Formatting.ResponseMappers
{
    public class ResponsesMapperProfile : Profile
    {
        public ResponsesMapperProfile()
        {
            Guid userId = default;
            CreateMap<Exercise, ExerciseCompactInternalModel>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Status, map => map.MapFrom(e => e
                    .Solutions
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.Status)
                    .Select(s => s.Status)
                    .FirstOrDefault()
                    ))
                .ForMember(r => r.ChallengeViewMode, map => map.MapFrom(c => c.Challenge.ViewMode));

            CreateMap<ExerciseCompactInternalModel, ExerciseCompactResponse>()
                .ForMember(ecim => ecim.Status, map => map.MapFrom(ecr => ecr.GetStatus()))
                .ForMember(ecim => ecim.HiddenStatus, map => map.MapFrom(ecr => ecr.GetHiddenStatus()));

            CreateMap<Exercise, ExerciseInfo>()
                .ForMember(r => r.Id, map => map.MapFrom(e => e.ExerciseID))
                .ForMember(r => r.Name, map => map.MapFrom(e => e.ExerciseName))
                .ForMember(r => r.Solutions, map => map.Ignore());


            CreateMap<User, LoginResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));
            CreateMap<User, UserInfoResponse>()
                .ForMember(r => r.StudentId, map => map.MapFrom(u => u.StudentID));

            CreateMap<Solution, SolutionInternalModel>()
                .ForMember(sim => sim.ChallengeViewMode, map => map.MapFrom(s => s.Exercise.Challenge.ViewMode));
            CreateMap<SolutionInternalModel, SolutionResponse>()
                .ForMember(sr => sr.Status, map => map.MapFrom(sim => sim.GetStatus()))
                .ForMember(sr => sr.HiddenStatus, map => map.MapFrom(sim => sim.GetHiddenStatus()));

            CreateMap<Challenge, ChallengeResponse>();

            CreateMap<ExerciseData, ExerciseDataResponse>();
            CreateMap<ExerciseData, ExerciseDataCompactResponse>();

            CreateMap<Solution, SolutionDumpView>()
                .ForMember(d => d.ExerciseName, map => map.MapFrom(s => s.Exercise.ExerciseName))
                .ForMember(d => d.UserId, map => map.MapFrom(s => s.User.StudentID))
                .ForMember(d => d.ExerciseScore, map => map.MapFrom(s => s.Exercise.Score));

            CreateMap<SolutionCheck, SolutionCheckResponse>();

            CreateMap<Course, CourseCompactResponse>()
                .ForMember(ccr => ccr.GroupsCount, map => map.MapFrom(c => c.GroupToCourses.Count));
            CreateMap<Course, CourseResponse>()
                .ForMember(cr => cr.Groups, map => map.MapFrom(c => c.GroupToCourses.Select(gtc => gtc.Group)));

            CreateMap<Group, GroupCompactResponse>();
            CreateMap<Group, GroupResponse>()
                .ForMember(gr => gr.Users, map => map.MapFrom(g => g.UserToGroups.Select(utg => utg.User)));
        }
    }
}
