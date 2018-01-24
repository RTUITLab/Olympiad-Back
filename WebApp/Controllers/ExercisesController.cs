﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApp.ViewModels;
using AutoMapper;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Exercises")]
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
        public List<Exercise> Get()
        {
            return applicationDbContext.Exercises.ToList();
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