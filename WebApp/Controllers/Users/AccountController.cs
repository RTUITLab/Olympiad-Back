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
using WebApp.Services.ReCaptcha;
using System.Net;
using Microsoft.Extensions.Logging;
using PublicAPI.Responses.Users;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IEmailSender emailSender;
        private readonly IRecaptchaVerifier recaptchaVerifier;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IMapper mapper,
            UserManager<User> userManager,
            IEmailSender emailSender,
            IRecaptchaVerifier recaptchaVerifier,
            ILogger<AccountController> logger)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.recaptchaVerifier = recaptchaVerifier;
            this.logger = logger;
        }

        [HttpGet]
        public Task<List<UserInfoResponse>> Get()
            => userManager
            .Users
            .ProjectTo<UserInfoResponse>()
            .ToListAsync();

        [HttpGet("{id}/{*token}")]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
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

            var recaptchaResult = await recaptchaVerifier.Check(model.RecaptchaToken, HttpContext.Connection.RemoteIpAddress.ToString());

            if (!recaptchaResult.Success)
            {
                logger.LogWarning($"Not acceppted user.");
                return BadRequest();
            }

            User userIdentity = mapper.Map<User>(model);

            var result = await userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            result = await userManager.AddToRoleAsync(userIdentity, "User");
            var token = await userManager.GenerateEmailConfirmationTokenAsync(userIdentity);
            var url = $"http://localhost:5000/api/Account/{userIdentity.Id}/{token}";
            await emailSender.SendEmailConfirm(model.Email, url);

            return new OkObjectResult("Account created");
        }
    }
}