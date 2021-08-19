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
using WebApp.Extensions;
using PublicAPI.Responses;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : AuthorizeController
    {

        private readonly IMapper mapper;
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
            IOptions<AccountSettings> options) : base(userManager)
        {
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.recaptchaVerifier = recaptchaVerifier;
            this.logger = logger;
            this.options = options;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ListResponse<UserInfoResponse>> Get(
            [MaxLength(100)] string match,
            [Range(0, int.MaxValue)] int offset = 0,
            [Range(1, 200)] int limit = 50)
        {
            var words = (match ?? "").ToUpper().Split(' ');
            var users = UserManager.Users;
            users = words.Aggregate(users, (usersCollection, matcher) => usersCollection.Where(
                u =>
                    u.FirstName.ToUpper().Contains(matcher) ||
                    u.Email.ToUpper().Contains(matcher) ||
                    u.StudentID.ToUpper().Contains(matcher)));
            var totalCount = await users.CountAsync();
            var result = await users
                .Skip(offset)
                .Take(limit)
                .OrderBy(u => u.FirstName)
                .ProjectTo<UserInfoResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return new ListResponse<UserInfoResponse> { Limit = limit, Total = totalCount, Offset = offset, Data = result };
        }

        [HttpGet("{id}/{*token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            var user = await UserManager.FindByIdAsync(id);
            var result = await UserManager.ConfirmEmailAsync(user, token);
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


        [HttpDelete("deleteUser/{studentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> DeleteUser(string studentId)
        {
            var targetIUser = await UserManager.Users.Where(u => u.StudentID == studentId).SingleAsync();
            await UserManager.DeleteAsync(targetIUser);
            return 1;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] RegistrationRequest model)
        {
            if (!options.Value.IsRegisterAvailable)
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

            var result = await UserManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            result = await UserManager.AddToRoleAsync(userIdentity, "User");
            //var token = await UserManager.GenerateEmailConfirmationTokenAsync(userIdentity);
            //var url = $"http://localhost:5000/api/Account/{userIdentity.Id}/{token}";
            //await emailSender.SendEmailConfirm(model.Email, url);

            return Ok();
        }

        [HttpPost("changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var user = await UserManager.FindByIdAsync(UserManager.GetUserId(User));

            var result = await UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var allClaims = await UserManager.GetClaimsAsync(user);
                var resetPasswordClaims = allClaims
                    .Where(c => c.Type == "reset_password" && c.Value == "need")
                    .ToList();
                if (resetPasswordClaims.Any())
                {
                    var removeClaimsResult = await UserManager.RemoveClaimsAsync(user, resetPasswordClaims);
                    logger.LogInformation($"User changed default password");
                }
                return Ok();
            }
            return BadRequest(result.Errors);
        }
    }
}