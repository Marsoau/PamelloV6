using Microsoft.AspNetCore.Mvc;

namespace PamelloV6.ClientASP.Controllers
{
    public class PlayerController : Controller
    {
        public IActionResult Index() {
            Console.WriteLine($"ckiezz: {Request.Cookies["token"]}");
            return View();
        }
    }
}
