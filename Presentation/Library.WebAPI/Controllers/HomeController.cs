using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
