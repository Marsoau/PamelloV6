using Microsoft.AspNetCore.Mvc;
using PamelloV6.ClientASP.Models;

namespace PamelloV6.ClientASP.Controllers
{
    public class PlayerController : Controller
    {
        public IActionResult Index() {
            var cToken = Request.Cookies["token"];
            if (cToken is null || !Guid.TryParse(cToken, out var token)) {
                return Redirect("/");
            }

            return View(new PlayerViewModel() {
                Token = token
            });
        }
    }
}
