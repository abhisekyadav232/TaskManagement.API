using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
