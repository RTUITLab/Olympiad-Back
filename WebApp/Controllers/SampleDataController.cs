using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using WebApp.Models.Responces;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public SampleDataController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Get()
        {
            var userId = Guid.Parse("5E3BAAFF-3E3A-4F0A-4508-08D5695FFE7F");

            var exerciseId = Guid.Parse("4C25F76A-681B-42AB-7E08-08D569610788");
            return Json(
                dbContext
                .Exercises
                .Select(E => new ExerciseListResponse
                {
                    Id = E.ExerciseID,
                    Name = E.ExerciseName,
                    Score = E.Score,
                    Status = E.Solution
                        .Where(S => S.UserId == userId)
                        .Select(S => S.Status)
                        .Max()
                }));
        }
    }
}
