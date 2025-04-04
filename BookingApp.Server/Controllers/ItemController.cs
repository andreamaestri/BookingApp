using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
