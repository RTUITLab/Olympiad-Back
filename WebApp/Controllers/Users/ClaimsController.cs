using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using PublicAPI.Requests.Account;
using PublicAPI.Responses.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Controllers.Users
{
    [Route("api/account/{userId:guid}/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ClaimsController : AuthorizeController
    {
        private readonly IMapper mapper;

        public ClaimsController(
            IMapper mapper,
            UserManager<User> userManager) : base(userManager)
        {
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClaimResponseObject>>> GetClaims(Guid userId)
        {
            var targetUser = await UserManager.FindByIdAsync(userId.ToString());
            if (targetUser == null)
            {
                return NotFound("User not found");
            }
            var userClaims = await UserManager.GetClaimsAsync(targetUser);
            return userClaims.OrderBy(c => c.Type).Select(c => mapper.Map<ClaimResponseObject>(c)).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<List<ClaimResponseObject>>> AddClaim(Guid userId, ClaimRequest data)
        {
            var targetUser = await UserManager.FindByIdAsync(userId.ToString());
            if (targetUser == null)
            {
                return NotFound("User not found");
            }
            var claimToAdd = new Claim(data.Type, data.Value);
            var result = await UserManager.AddClaimAsync(targetUser, claimToAdd);
            if (result.Succeeded)
            {
                return await GetClaims(userId);
            }
            else
            {
                return StatusCode(500, "Unhandled error");
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<ClaimResponseObject>>> RemoveClaim(Guid userId, [FromQuery] ClaimRequest data)
        {
            var targetUser = await UserManager.FindByIdAsync(userId.ToString());
            if (targetUser == null)
            {
                return NotFound("User not found");
            }

            var result = await UserManager.RemoveClaimAsync(targetUser, new Claim(data.Type, data.Value));

            if (result.Succeeded)
            {
                return await GetClaims(userId);
            }
            else
            {
                return StatusCode(500, "Unhandled error");
            }
        }
    }
}
