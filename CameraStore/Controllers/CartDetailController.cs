using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class CartDetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
