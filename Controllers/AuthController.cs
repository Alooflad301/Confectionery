using Microsoft.AspNetCore.Mvc;

namespace Confectionery.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
