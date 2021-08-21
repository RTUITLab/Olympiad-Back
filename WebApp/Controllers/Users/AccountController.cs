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
using Npgsql;
using WebApp.Services;
using PublicAPI.Responses.Account;

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

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserInfoResponse>> Get(Guid userId)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound("User not found");
            }
            return mapper.Map<UserInfoResponse>(user);
        }

        [HttpPut("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserInfoResponse>> UpdateAccountInfo(
            Guid userId,
            [FromBody] UpdateAccountInfoRequest model)
        {
            var targetUser = await UserManager.FindByIdAsync(userId.ToString());
            if (targetUser == null)
            {
                return NotFound("User not found");
            }

            mapper.Map(model, targetUser);
            try
            {
                var updateResul = await UserManager.UpdateAsync(targetUser);
                if (!updateResul.Succeeded)
                {
                    logger.LogError($"Can't update user info {updateResul}");
                    return StatusCode(500, "Unhandled error");
                }
                return mapper.Map<UserInfoResponse>(targetUser);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException psex &&
                psex.SqlState == PostgresErrorCodes.UniqueViolation &&
                psex.ConstraintName.Contains(nameof(targetUser.StudentID)))
            {
                ModelState.AddModelError(nameof(targetUser.StudentID), $"StudentID {targetUser.StudentID} already exists");
                return BadRequest(ModelState);
            }
        }

        [HttpGet("confirmEmail/{id}/{*token}")]
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


        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            var targetIUser = await UserManager.FindByIdAsync(userId.ToString());
            if (targetIUser == null)
            {
                return NotFound("User not found");
            }
            var deleteResult = await UserManager.DeleteAsync(targetIUser);
            if (!deleteResult.Succeeded)
            {
                logger.LogError($"Can't delete user {deleteResult}");
                return StatusCode(500);
            }
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserInfoResponse>> Post([FromBody] RegistrationRequest model)
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
            try
            {

                var result = await UserManager.CreateAsync(userIdentity, model.Password);

                if (!result.Succeeded)
                    return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

                result = await UserManager.AddToRoleAsync(userIdentity, "User");
                //var token = await UserManager.GenerateEmailConfirmationTokenAsync(userIdentity);
                //var url = $"http://localhost:5000/api/Account/{userIdentity.Id}/{token}";
                //await emailSender.SendEmailConfirm(model.Email, url);

                return mapper.Map<UserInfoResponse>(userIdentity);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException psex &&
                psex.SqlState == PostgresErrorCodes.UniqueViolation &&
                psex.ConstraintName.Contains(nameof(userIdentity.StudentID)))
            {
                ModelState.AddModelError(nameof(userIdentity.StudentID), $"StudentID {userIdentity.StudentID} already exists");
                return BadRequest(ModelState);
            }
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

        [HttpPost("adminChangePassword/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NewPasswordGeneratedResponse>> AdminChangePassword(
            Guid userId,
            [FromServices] UserPasswordGenerator userPasswordGenerator)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound("User nor found");
            }
            var removePasswordResult = await UserManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                logger.LogError($"Can't remove password");
                return StatusCode(500, "Unhandled error");
            }
            var password = userPasswordGenerator.GeneratePassword();
            var addPasswordResult = await UserManager.AddPasswordAsync(user, password);

            if (!addPasswordResult.Succeeded)
            {
                logger.LogError($"Can't add new password");
                return StatusCode(500, "Unhandled error");
            }

            return new NewPasswordGeneratedResponse { NewPassword = password };
        }
    }
}