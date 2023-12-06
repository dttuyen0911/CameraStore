using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Cart> carts = _dbContext.Carts.ToList();
            return View(carts);
        }
        public IActionResult addtoCartStore(int id)
        {
            IEnumerable<Cart> carts = _dbContext.Carts.ToList();
            return View(carts);
        }
    }
}
