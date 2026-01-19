using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
