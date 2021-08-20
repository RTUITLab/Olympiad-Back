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
using Olympiad.Services.JWT;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthController> logger;

        public AuthController(UserManager<User> userManager, IJwtFactory jwtFactory, IMapper mapper, ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
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
            var user = await context.Students.SingleOrDefaultAsync(c => c.StudentID == credentials.Login || c.UserName == credentials.Login);
            if (user == null) return BadRequest($"Can't find user with StudentID or UserName \"{credentials.Login}\"");
            if (!await _userManager.CheckPasswordAsync(user, credentials.Password))
            {
                return Unauthorized("invalid username or password");
            }

            var loginInfo = await GenerateResponse(user);
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == "reset_password"))
            {
                _ = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(async (t) => await notifyUsersService.SendInformationMessageToUser(user.Id, defaultUserSettings.Value.ResetPasswordWarningText));
            }

            return loginInfo;
        }


        [HttpGet("getme")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<GetMeResult>> Get()
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user == null)
            {
                logger.LogWarning($"Correct JWT but user not found userId: {_userManager.GetUserId(User)}");
                return Forbid("Bearer");
            }
            var plainResult = await GenerateResponse(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
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
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }
            var token = await GenerateAccessToken(user);
            return new TokenResponse { Token = token };

        }


        private async Task<LoginResponse> GenerateResponse(User user)
        {
            var loginInfo = mapper.Map<LoginResponse>(user);
            loginInfo.Token = await GenerateAccessToken(user);
            return loginInfo;
        }

        private async Task<string> GenerateAccessToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return _jwtFactory.GenerateToken(user, userRoles);
        }

    }
}