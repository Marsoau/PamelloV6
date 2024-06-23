using Microsoft.AspNetCore.Mvc;
using PamelloV6.ClientASP.Models;

namespace PamelloV6.ClientASP.Controllers
{
    public class PlayerController : Controller
    {
        public IActionResult Index() {
            return View(new PlayerViewModel() {
                Token = Guid.Empty
            });
        }
    }
}
