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
using Microsoft.Extensions.Options;
using WebApp.Models.Settings;

namespace WebApp.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : Controller
    {

        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IEmailSender emailSender;
        private readonly IRecaptchaVerifier recaptchaVerifier;
        private readonly ILogger<AccountController> logger;
        private readonly IOptions<AccountSettings> options;

        public AccountController(
            IMapper mapper,
            UserManager<User> userManager,
            IEmailSender emailSender,
            IRecaptchaVerifier recaptchaVerifier,
            ILogger<AccountController> logger,
            IOptions<AccountSettings> options)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.recaptchaVerifier = recaptchaVerifier;
            this.logger = logger;
            this.options = options;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public Task<List<UserInfoResponse>> Get()
            => userManager
            .Users
            .ProjectTo<UserInfoResponse>()
            .ToListAsync();

        [HttpGet("{id}/{*token}")]
        [AllowAnonymous]
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

        [HttpGet("isRegisterAvailable")]
        [AllowAnonymous]
        public bool IsRegisterAvailable()
        {
            return options.Value.IsRegisterAvailable;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]RegistrationRequest model)
        {
            if (!ModelState.IsValid || !options.Value.IsRegisterAvailable)
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