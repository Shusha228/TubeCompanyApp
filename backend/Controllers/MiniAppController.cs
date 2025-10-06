using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class MiniAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MiniAppController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("/miniapp")]
        public IActionResult ServeMiniApp()
        {
            var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
            return Redirect(frontendUrl);
        }

        [HttpGet("/")]
        public IActionResult RedirectToMiniApp()
        {
            return Redirect("/miniapp");
        }
    }
}