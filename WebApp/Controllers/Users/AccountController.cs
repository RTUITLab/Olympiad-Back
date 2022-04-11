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
using PublicAPI.Responses.Account;
using System.Security.Claims;
using Olympiad.Shared;
using PublicAPI.Requests.Account;
using OneOf;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.Links;
using WebApp.Services;
using WebApp.Formatting;

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
        public async Task<ListResponseWithMatch<UserInfoResponse>> Get(
            [MaxLength(100)] string match,
            [FromQuery] ListQueryParams listQuery,
            [FromQuery, ModelBinder(typeof(ClaimRequestBinder))] IEnumerable<ClaimRequest> targetClaims,
            [FromServices] ApplicationDbContext context)
        {
            using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

            var users = UserManager.Users
                .AsNoTracking()
                .FindByMatch(match)
                .FindByClaims(targetClaims);

            var totalCount = await users.CountAsync();
            var result = await users
                .OrderBy(u => u.FirstName)
                .Skip(listQuery.Offset)
                .Take(listQuery.Limit)
                .ProjectTo<UserInfoResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return new ListResponseWithMatch<UserInfoResponse> { Limit = listQuery.Limit, Total = totalCount, Offset = listQuery.Offset, Match = match, Data = result };
        }

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "Admin,ResultsViewer")]
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

        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        [Obsolete("Move generating user logic to Service")]
        public async Task<ActionResult> GenerateUser(
            [FromBody] GenerateUserRequest model,
            [FromServices] ApplicationDbContext dbContext)
        {
            var createUserResult = await CreateUser(new CreateUserDataModel
            {
                StudentID = model.ID,
                Email = $"{model.ID}@{options.Value.EmailDomain}",
                FirstName = model.Name,
                Password = model.Password,
                LastName = ""
            });
            if (createUserResult.IsT1)
            {
                var (statusCode, value) = createUserResult.AsT1;
                if (statusCode == 400 && value is ModelStateDictionary modelState)
                {
                    return BadRequest(modelState);
                }
                return StatusCode(statusCode, value);
            }
            if (model.Claims is null)
            {
                return Ok();
            }
            var user = createUserResult.AsT0;

            foreach (var challengeId in model
                .Claims.Where(c => c.Type == DefaultClaims.AddToChallenge.Type)
                .Select(c => c.Value)
                .Select(c => (success: Guid.TryParse(c, out var id), id))
                .Where(c => c.success)
                .Select(c => c.id))
            {
                try
                {
                    dbContext.Add(new UserToChallenge
                    {
                        ChallengeId = challengeId,
                        UserId = user.Id
                    });
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Can't add user {user} to challenge {challenge}", user.Id, challengeId);
                }
            }

            foreach (var claim in model.Claims)
            {
                var claimToAdd = new Claim(claim.Type, claim.Value);
                var result = await UserManager.AddClaimAsync(user, claimToAdd);
                if (!result.Succeeded)
                {
                    logger.LogError($"Can't add claim {claimToAdd} to user {user.Id} {user.StudentID}: {result}");
                    return StatusCode(500, "Unexpected error");
                }
            }
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserInfoResponse>> Post([FromBody] RegistrationRequest model)
        {
            if (!options.Value.IsRegisterAvailable)
            {
                return Conflict("resgistration is not available");
            }

            var recaptchaResult = await recaptchaVerifier.Check(model.RecaptchaToken, HttpContext.Connection.RemoteIpAddress.ToString());

            if (!recaptchaResult.Success)
            {
                logger.LogWarning($"Not acceppted user.");
                return BadRequest();
            }

            var createUserResult = await CreateUser(model);
            if (createUserResult.IsT0)
            {
                return mapper.Map<UserInfoResponse>(createUserResult.AsT0);
            }
            else
            {
                var (statusCode, value) = createUserResult.AsT1;
                if (statusCode == 400 && value is ModelStateDictionary modelState)
                {
                    return BadRequest(modelState);
                }
                return StatusCode(statusCode, value);
            }
        }

        // TODO: Extract that method to service
        [Obsolete("Extract that method to service")]
        private async Task<OneOf<User, (int statusCode, object content)>> CreateUser(CreateUserDataModel createUserModel)
        {
            User userIdentity = mapper.Map<User>(createUserModel);
            try
            {

                var result = await UserManager.CreateAsync(userIdentity, createUserModel.Password);

                if (!result.Succeeded)
                {
                    Errors.AddErrorsToModelState(result, ModelState);
                    return (400, ModelState);
                }

                result = await UserManager.AddToRoleAsync(userIdentity, RoleNames.USER);
                if (!result.Succeeded)
                {
                    logger.LogError($"Can't add user to role {result}");
                    return (500, "Unexpected error");
                }

                result = await UserManager.AddClaimAsync(userIdentity, DefaultClaims.NeedResetPassword.Claim);
                if (!result.Succeeded)
                {
                    logger.LogError($"Can't add default claim {result}");
                    return (500, "Unexpected error");
                }

                //var token = await UserManager.GenerateEmailConfirmationTokenAsync(userIdentity);
                //var url = $"http://localhost:5000/api/Account/{userIdentity.Id}/{token}";
                //await emailSender.SendEmailConfirm(model.Email, url);

                return userIdentity;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException psex &&
                psex.SqlState == PostgresErrorCodes.UniqueViolation &&
                psex.ConstraintName.Contains(nameof(userIdentity.StudentID)))
            {
                ModelState.AddModelError(nameof(userIdentity.StudentID), $"StudentID {userIdentity.StudentID} already exists");
                return (400, ModelState);
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
                    .Where(c => c.Type == DefaultClaims.NeedResetPassword.Type && c.Value == DefaultClaims.NeedResetPassword.Value)
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



            var allClaims = await UserManager.GetClaimsAsync(user);
            var resetPasswordClaims = allClaims
                .Where(c => c.Type == DefaultClaims.NeedResetPassword.Type && c.Value == DefaultClaims.NeedResetPassword.Value)
                .ToList();
            if (!resetPasswordClaims.Any())
            {
                var addNeedChangePasswordResult = await UserManager.AddClaimAsync(user, DefaultClaims.NeedResetPassword.Claim);
                if (!addNeedChangePasswordResult.Succeeded)
                {
                    logger.LogWarning($"Can't add claim about changing password {addNeedChangePasswordResult}");
                }
            }

            return new NewPasswordGeneratedResponse { NewPassword = password };
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId:guid}/loginEvents")]
        public async Task<ActionResult<List<LoginEventResponse>>> GetLoginEvents(Guid userId, 
            [FromServices] ApplicationDbContext dbContext)
        {
            return await dbContext.LoginEvents
                .Where(le => le.UserId == userId)
                .OrderByDescending(h => h.LoginTime)
                .Take(30)
                .Select(h => mapper.Map<LoginEventResponse>(h))
                .ToListAsync();
        }

        [Authorize(Roles = RoleNames.ADMIN + "," + RoleNames.RESULTS_VIEWER)]
        [HttpGet("claims")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetClaimTypes([FromServices] ApplicationDbContext context)
        {
            return (await context
                .UserClaims
                .Select(c => new { Type = c.ClaimType, Value = c.ClaimValue })
                .Distinct()
                .ToListAsync())
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToList());
        }
    }
}