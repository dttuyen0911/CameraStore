using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Order> orders = _dbContext.Orders.ToList();
            return View(orders);
        }
        public IActionResult createOrder(int id)
        {
            IEnumerable<Order> orders = _dbContext.Orders.ToList();
            return View(orders);
        }
    }
}
