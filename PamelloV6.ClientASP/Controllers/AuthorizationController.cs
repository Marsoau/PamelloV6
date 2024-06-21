using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace PamelloV6.ClientASP.Controllers
{
    [EnableCors("basePolicy")]
    public class AuthorizationController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}
