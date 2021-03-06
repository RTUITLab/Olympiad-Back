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

        [HttpGet]
        [Route("{exerciseId}")]
        public Task<List<ExerciseDataCompactResponse>> Get(Guid exerciseId)
        {
            return context
                .TestData
                .Where(p => p.ExerciseDataGroup.ExerciseId == exerciseId)
                .Where(p => p.ExerciseDataGroup.IsPublic)
                .ProjectTo<ExerciseDataCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpGet]
        [Route("all/{exerciseId}")]
        [Authorize(Policy = "Executor")]
        public Task<List<ExerciseDataResponse>> GetAll(Guid exerciseId)
        {
            return context
                .TestData
                .Where(p => p.ExerciseDataGroup.ExerciseId == exerciseId)
                .ProjectTo<ExerciseDataResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}