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
using WebApp.Services.Interfaces;
using PublicAPI.Requests;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IEmailSender emailSender;

        public AccountController(
            IMapper mapper,
            UserManager<User> userManager,
            IEmailSender emailSender
            )
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        [HttpGet]
        [Route("{id}/{*token}")]
        public async Task<IActionResult> Get(string id, string token)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Content("Ваш email подтвержден");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegistrationRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User userIdentity = mapper.Map<User>(model);

            var result = await userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            result = await userManager.AddToRoleAsync(userIdentity, "User");
            var token = await userManager.GenerateEmailConfirmationTokenAsync(userIdentity);
            var url = $"http://localhost:5000/api/Account/{userIdentity.Id}/{token}";
            await emailSender.SendEmailConfirm(model.Email, url);

            return new OkObjectResult("Account created");
        }
    }
}