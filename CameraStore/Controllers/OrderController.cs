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
            int userId = Convert.ToInt32(User.Identity.Name);
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            if (customer != null)
            {
                ViewBag.FullName = customer.fullname;
            }
            else
            {
                ViewBag.FullName = "Unknown";
            }
            return View(orders);
        }
        public IActionResult createOrder(int id)
        {
            IEnumerable<Order> orders = _dbContext.Orders.ToList();
            return View(orders);
        }
    }
}
