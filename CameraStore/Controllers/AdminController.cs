using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
