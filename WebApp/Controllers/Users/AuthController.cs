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

        public AuthController(UserManager<User> userManager, IJwtFactory jwtFactory, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            this.mapper = mapper;
            this.context = context;
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
            if (!await _userManager.CheckPasswordAsync(user, credentials.Password)
                //|| await _userManager.IsEmailConfirmedAsync(userToVerify)
                )
            {
                return Unauthorized("invalid username or password");
            }
            
            var loginInfo = await GenerateResponse(user);
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == "reset_password"))
            {
                _ = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(async (t) => await notifyUsersService.SendInformationMessageToUser(user.Id, defaultUserSettings.Value.ResetPasswordWarningText));
            }

            return Ok(loginInfo);
        }


        [HttpGet("getme")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user == null)
                return StatusCode(403);
            return Json(await GenerateResponse(user));
        }

        private async Task<LoginResponse> GenerateResponse(User user)
        {
            var loginInfo = mapper.Map<LoginResponse>(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            loginInfo.Token = _jwtFactory.GenerateToken(user, userRoles);
            return loginInfo;
        }
    }
}