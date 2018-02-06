using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using WebApp.Auth;
using WebApp.Helpers;
using WebApp.Models.Responces;
using WebApp.ViewModels;

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
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<User> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            this.mapper = mapper;
            this.context = context;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (identity, user) = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName);

            var loginInfo = mapper.Map<LoginResponse>(user);
            loginInfo.Token = jwt;

            var sum = context
                 .Exercises
                 .Where(E => E.Solution.Any(S => S.Status == SolutionStatus.Sucessful && S.UserId == user.Id))
                 .Sum(E => E.Score);

            loginInfo.Token = jwt;
            loginInfo.TotalScore = sum;
            return Json(loginInfo);
        }

        private async Task<(ClaimsIdentity, User)> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return (null, null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return (null, null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password)
                //&& await _userManager.IsEmailConfirmedAsync(userToVerify)
                )
            {
                return (_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id.ToString()), userToVerify);
            }

            // Credentials are invalid, or account doesn't exist
            return (null, null);
        }
    }
}