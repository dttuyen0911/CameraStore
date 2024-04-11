using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CameraStore.Controllers
{
    [Authorize]
    public class OrderDetailController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public OrderDetailController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
        }
        [Authorize(Policy = "EmployeePolicy")]
        [Authorize(Policy = "OwnerPolicy")]
        public IActionResult Index(int ?id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderdetails = _dbContext.OrderDetails
                .Where(od => od.orderID == id)
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

            var orderDetails = _dbContext.OrderDetails
                .Include(od => od.Order)
                .ThenInclude(o => o.Customer)
                .Include(od => od.Product)
                .Where(od => od.Order.customerID == userId)
                .ToList();

            if (orderDetails == null || orderDetails.Count == 0)
            {
                ViewBag.Message = "No order found";
            }

            return View(orderDetails);
        }
        public IActionResult orderDetail(int ?id)
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
            if (id == null)
            {
                return NotFound(); // Trả về trang 404 nếu không có id được cung cấp
            }

            var orderDetail = _dbContext.OrderDetails
                .Include(od => od.Order)
                .ThenInclude(o => o.Customer)
                .Include(od => od.Product)
                .FirstOrDefault(od => od.Order.orderID == id);

            if (orderDetail == null)
            {
                return NotFound(); // Trả về trang 404 nếu không tìm thấy thông tin đơn hàng
            }
            return View("orderDetail", new List<OrderDetail> { orderDetail });
        }
    }
}
