using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AuthorizeController : Controller
    {
        protected readonly UserManager<User> UserManager;

        public AuthorizeController(UserManager<User> userManager)
        {
            this.UserManager = userManager;
        }

        protected Guid UserId => Guid.Parse(UserManager.GetUserId(User));
        protected Task<User> CurrentUser() => UserManager.GetUserAsync(User);
    }
}
