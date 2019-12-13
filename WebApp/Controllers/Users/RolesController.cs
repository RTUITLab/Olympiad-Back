using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using WebApp.Models;

namespace WebApp.Controllers.Users
{
    [Route("api/roles")]
    [Authorize(Roles = "User")]
    public class RolesController : AuthorizeController
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public RolesController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager) : base(userManager)
        {
            this.roleManager = roleManager;
        }

        [HttpGet("")]
        public Task<List<string>> RolesFor()
        {
            return roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        [HttpGet("{userId:guid}")]
        public async Task<IEnumerable<string>> RolesFor(Guid userId)
        {
            return await UserManager.GetRolesAsync(await CurrentUser());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userId:guid}/{role}")]
        public async Task<string> AddRole(Guid userId, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                throw StatusCodeException.BadRequest();
            var result = await UserManager.AddToRoleAsync(await CurrentUser(), role);
            if (result.Succeeded)
                return role;
            throw StatusCodeException.BadRequest(string.Join(',', result.Errors));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId:guid}/{role}")]
        public async Task<string> DemoveFromRole(Guid userId, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                throw StatusCodeException.BadRequest();
            var result = await UserManager.RemoveFromRoleAsync(await CurrentUser(), role);
            if (result.Succeeded)
                return role;
            throw StatusCodeException.BadRequest(string.Join(',', result.Errors));
        }
    }
}
