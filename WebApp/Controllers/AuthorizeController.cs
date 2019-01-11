using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AuthorizeController : Controller
    {
        protected readonly UserManager<User> UserManager;

        public DateTime Now { get; }

        public AuthorizeController(UserManager<User> userManager)
        {
            UserManager = userManager;
            Now = DateTime.UtcNow;
        }

        protected Guid UserId => Guid.Parse(UserManager.GetUserId(User));
        protected Task<User> CurrentUser() => UserManager.GetUserAsync(User);
        protected bool IsInRole(string roleName) => User.HasClaim(ClaimTypes.Role, roleName);
        protected bool IsAdmin => IsInRole("Admin");
    }
}
