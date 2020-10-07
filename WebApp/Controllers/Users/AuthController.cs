using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Models.Solutions;
using Newtonsoft.Json;
using PublicAPI.Requests;
using PublicAPI.Responses;
using Olympiad.Shared.Models;
using WebApp.Auth;
using WebApp.Helpers;
using WebApp.ViewModels;
using WebApp.Services;
using WebApp.Models.Settings;

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
        public async Task<IActionResult> Post(
            [FromBody] CredentialsRequest credentials,
            [FromServices] NotifyUsersService notifyUsersService,
            [FromServices] IOptions<DefaultUserSettings> defaultUserSettings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await context.Students.SingleOrDefaultAsync(c => c.StudentID == credentials.Login || c.UserName == credentials.Login);
            if (user == null) return BadRequest();
            if (!await _userManager.CheckPasswordAsync(user, credentials.Password)
                //|| await _userManager.IsEmailConfirmedAsync(userToVerify)
                )
            {
                return Unauthorized("invalid username or password");
            }
            
            var loginInfo = await GenerateResponse(user);
            if (defaultUserSettings.Value.StudentId == user.StudentID && credentials.Password == defaultUserSettings.Value.Password)
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

        private async Task<LoginResponse> GenerateResponse(User user, string token = "")
        {
            var loginInfo = mapper.Map<LoginResponse>(user);
            loginInfo.Token = token;
            var userRoles = await _userManager.GetRolesAsync(user);
            var identity = _jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString(), userRoles.ToArray());

            loginInfo.Token = await Tokens.GenerateJwt(identity, _jwtFactory, user.UserName);
            return loginInfo;
        }
    }
}