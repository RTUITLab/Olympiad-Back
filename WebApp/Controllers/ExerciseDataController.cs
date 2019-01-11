using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Exercises;
using PublicAPI.Requests;
using PublicAPI.Responses;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/ExerciseData")]
    public class ExerciseDataController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ExerciseDataController(
            ApplicationDbContext context, 
            UserManager<User> userManager,
            IMapper mapper) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpPost]
        [Route("{exerciseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ExerciseDataResponse> Post(Guid exerciseId, [FromBody]ExerciseDataRequest exerciseDataIn)
        {
            if (exerciseDataIn == null || string.IsNullOrEmpty(exerciseDataIn.InData) || string.IsNullOrEmpty(exerciseDataIn.OutData))
            {
                throw StatusCodeException.BadRequest("No exercise data in request");
            }

            if (!context.Exercises.Any(e => e.ExerciseID == exerciseId))
            {
                throw StatusCodeException.BadRequest("No target exercise");
            }

            var exerciseData = new ExerciseData
            {
                ExerciseId = exerciseId,
                InData = exerciseDataIn.InData,
                OutData = exerciseDataIn.OutData,
                IsPublic = exerciseDataIn.IsPublic
            };
            context.TestData.Add(exerciseData);
            await context.SaveChangesAsync();

            return mapper.Map<ExerciseDataResponse>(exerciseData);
        }

        [HttpGet]
        [Route("{exerciseId}")]
        public Task<List<ExerciseDataCompactResponse>> Get(Guid exerciseId)
        {
            return context
                .TestData
                .Where(p => p.ExerciseId == exerciseId)
                .Where(p => p.IsPublic)
                .ProjectTo<ExerciseDataCompactResponse>()
                .ToListAsync();
        }


        [HttpGet]
        [Route("all/{exerciseId}")]
        [Authorize(Roles = "Admin, Executor")]
        public Task<List<ExerciseDataResponse>> GetAll(Guid exerciseId)
        {
            return context
                .TestData
                .Where(p => p.ExerciseId == exerciseId)
                .ProjectTo<ExerciseDataResponse>()
                .ToListAsync();
        }



        [HttpPut]
        [Route("{exerciseDataId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ExerciseDataResponse> Put(Guid exerciseDataId, [FromBody]ExerciseDataRequest exerciseDataIn)
        {
            if (exerciseDataIn == null || string.IsNullOrEmpty(exerciseDataIn.InData) || string.IsNullOrEmpty(exerciseDataIn.OutData))
            {
                throw StatusCodeException.BadRequest("No exercise data in request");
            }

            var testData = await context.TestData.SingleOrDefaultAsync(e => e.Id == exerciseDataId);
            if (testData == null)
            {
                throw StatusCodeException.BadRequest("No target exercise");
            }
            testData.InData = exerciseDataIn.InData;
            testData.OutData = exerciseDataIn.OutData;
            testData.IsPublic = exerciseDataIn.IsPublic;

            await context.SaveChangesAsync();

            return mapper.Map<ExerciseDataResponse>(testData);
        }

    }
}