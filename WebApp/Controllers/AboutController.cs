using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PublicAPI.Responses;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AboutController : ControllerBase
    {
        public ActionResult<AboutResponse> Get([FromServices] IConfiguration configuration)
        {
            return new AboutResponse
            {
                BuildNumber = configuration.GetValue<string>("About:BuildNumber") ?? "no-build-number"
            };
        }
    }
}
