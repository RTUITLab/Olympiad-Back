using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Olympiad.Services.UserSolutionsReport;
using Olympiad.Shared;
using Olympiad.Shared.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApp.Controllers
{ 
    
    [Route("api/reports")]
    public class ReportsController : AuthorizeController
    {
        public ReportsController(UserManager<User> userManager) : base(userManager)
        {
        }
        [Produces("text/plain")]
        [HttpGet("challenge/{challengeId:guid}/{studentId}")]
        [Authorize(Roles = RoleNames.RESULTS_VIEWER)]
        public async Task<ActionResult<string>> GetChallengeReportForUser(
            [FromRoute] Guid challengeId, 
            [FromRoute][Required] string studentId, 
            [FromQuery] UserSolutionsReportOptions options,
            [FromServices] UserSolutionsReportCreator reportCreator)
        {
            return await reportCreator.CreateMarkdownReport(studentId, challengeId, options);
        }
    }
}
