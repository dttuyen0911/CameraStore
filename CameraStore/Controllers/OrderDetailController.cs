using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class OrderDetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
