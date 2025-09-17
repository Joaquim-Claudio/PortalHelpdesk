using Microsoft.AspNetCore.Mvc;

namespace PortalHelpdesk.Controllers
{
    public class HomeController : ControllerBase
    {

        public IActionResult Index()
        {
            return Ok();
        }
    }
}
