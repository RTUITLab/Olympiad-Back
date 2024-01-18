using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PublicAPI.Responses;
using PublicAPI.Responses.SupportedRuntimes;
using System.Threading.Tasks;
using WebApp.Services;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AboutController : ControllerBase
{
    [HttpGet]
    public ActionResult<AboutResponse> Get([FromServices] IConfiguration configuration)
    {
        return new AboutResponse
        {
            BuildNumber = configuration.GetValue<string>("About:BuildNumber") ?? "no-build-number"
        };
    }
    [HttpGet("supportedruntimes")]
    public async Task<ActionResult<GetSupportedRuntimesResponse>> GetSupportedRuntimes([FromServices] ISupportedRuntimesService supportedRuntimes)
    {
        return new GetSupportedRuntimesResponse(await supportedRuntimes.GetSupportedRuntimes());
    }
}
