using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models.Responces;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Exercises")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ExercisesController : Controller
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext applicationDbContext;

        public ExercisesController(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            this.applicationDbContext = applicationDbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("{exerciseId}")]
        public IActionResult Get(Guid exerciseId)
        {
            return Json(applicationDbContext.Exercises.FirstOrDefault(P => P.ExerciseID == exerciseId));
        }

        [HttpGet]
        public IActionResult Get()
        {
            var exercises = applicationDbContext
                .Exercises
                .Select(mapper.Map<ExerciseResponce>);
            return Json(exercises);
        }

        [HttpPost]
        public void Post([FromBody] ExercisesViewModel model)
        {
            var exeIdentity = mapper.Map<Exercise>(model);
            applicationDbContext.Exercises.Add(exeIdentity);
            applicationDbContext.SaveChanges();
        }
    }
}