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

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<string>>> GetRoles(Guid userId)
        {
            var user = await GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(await UserManager.GetRolesAsync(user));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userId:guid}/{role}")]
        public async Task<ActionResult<IList<string>>> AddToRole(Guid userId, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                throw StatusCodeException.BadRequest();
            var targetUser = await GetUser(userId);
            if (targetUser == null)
            {
                return NotFound("User not found");
            }

            var result = await UserManager.AddToRoleAsync(targetUser, role);
            if (result.Succeeded)
                return Ok(await UserManager.GetRolesAsync(targetUser));

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId:guid}/{role}")]
        public async Task<ActionResult<IList<string>>> RemoveFromRole(Guid userId, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                return NotFound("Role not found");

            var targetUser = await GetUser(userId);
            if (targetUser == null)
            {
                return NotFound("User not found");
            }

            var result = await UserManager.RemoveFromRoleAsync(targetUser, role);
            if (result.Succeeded)
                return Ok(await UserManager.GetRolesAsync(targetUser));

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }
    }
}
