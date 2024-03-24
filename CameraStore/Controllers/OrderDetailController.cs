using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CameraStore.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(int ?id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var orderdetails = _dbContext.OrderDetails
                .Where(cd => cd.orderID == id)
                .Include(cd => cd.proID)
                .ToList();
            return View(orderdetails);
        }
        public IActionResult viewOrder()
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
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
            var order = _dbContext.Orders
              .Include(od => od.orderdetails)
              .ThenInclude(cd => cd.Product)
              .FirstOrDefault(c => c.customerID == int.Parse(customerId));
            if (order == null || order.orderdetails.Count == 0)
            {
                ViewBag.Message = "No order in irder";
                return View();
            }
            var orderDetails = order.orderdetails.ToList();

            return View(orderDetails);

        }
    }
}
