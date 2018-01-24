using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext dbContext;

        public AccountController(IMapper mapper, UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = mapper.Map<User>(model);

            var result = await  userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            //await dbContext.Students.AddAsync(new User{ StudentID = model.StudentID,});
            //await dbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
    }
}