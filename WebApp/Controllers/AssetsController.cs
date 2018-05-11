using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly IHostingEnvironment environment;

        public  AssetsController(IHostingEnvironment environment)
        {
            this.environment = environment;
        }
        [HttpPost("{*imagename}")]
        public void Post(IFormFile file) 
        {
        }
    }
}