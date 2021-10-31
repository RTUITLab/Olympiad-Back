using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using PublicAPI.Requests;
using PublicAPI.Responses;
using WebApp.Services;
using WebApp.Models.Settings;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Olympiad.Shared;
using Olympiad.Services.Authorization;
using Models.UserModels;
using WebApp.Extensions;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUserAuthorizationService authorizationService;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthController> logger;

        public AuthController(UserManager<User> userManager, IUserAuthorizationService authorizationService, IMapper mapper, ApplicationDbContext context, ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.authorizationService = authorizationService;
            this.mapper = mapper;
            this.context = context;
            this.logger = logger;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Post(
            [FromBody] CredentialsRequest credentials,
            [FromServices] NotifyUsersService notifyUsersService,
            [FromServices] IOptions<DefaultUsersSettings> defaultUserSettings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await authorizationService.GetJwtTokenForUser(credentials.Login, credentials.Password);
            if (result.IsT1)
            {
                return Unauthorized();
            }
            var (userToken, user) = result.AsT0;
            var loginInfo = GenerateResponse(user, userToken);
            var claims = await userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == DefaultClaims.NeedResetPassword.Type))
            {
                _ = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(async (t) => await notifyUsersService.SendInformationMessageToUser(user.Id, defaultUserSettings.Value.ResetPasswordWarningText));
            }


            var loginEventRecord = new LoginEvent
            {
                UserId = user.Id,
                LoginTime = DateTimeOffset.UtcNow,
                IP = HttpContext.GetClientIP(),
                UserAgent = HttpContext.GetClientUserAgent(),
            };
            
            context.LoginEvents.Add(loginEventRecord);
            await context.SaveChangesAsync();

            return loginInfo;
        }


        [HttpGet("getme")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<GetMeResult>> Get()
        {
            var userId = Guid.Parse(userManager.GetUserId(User));
            var result = await authorizationService.GetJwtTokenForUser(userId);

            if (result.IsT1)
            {
                logger.LogWarning($"Correct JWT but user not found userId: {userId}");
                return Forbid("Bearer");
            }

            var (accessToken, user) = result.AsT0;
            var plainResult = GenerateResponse(user, accessToken);
            var claims = await userManager.GetClaimsAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            claims = claims.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r))).ToList();

            var totalResult = new GetMeResult
            {
                Id = plainResult.Id,
                FirstName = plainResult.FirstName,
                Email = plainResult.Email,
                Token = plainResult.Token,
                StudentId = plainResult.StudentId,
                Claims = claims.GroupBy(c => c.Type).ToDictionary(c => c.Key, g => g.Select(c => c.Value).ToArray())
            };
            return totalResult;
        }

        [HttpGet("gettokenforuser/{userId:guid}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<TokenResponse>> GetTokenForUser(Guid userId)
        {
            var result = await authorizationService.GetJwtTokenForUser(userId);
            if (result.IsT1)
            {
                return NotFound("User not found");
            }
            return new TokenResponse { Token = result.AsT0.accessToken };
        }


        private LoginResponse GenerateResponse(User user, string accessToken)
        {
            var loginInfo = mapper.Map<LoginResponse>(user);
            loginInfo.Token = accessToken;
            return loginInfo;
        }
    }
}