using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CameraStore.Controllers
{
    public class StatisticalController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public StatisticalController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các đơn hàng chi tiết từ cơ sở dữ liệu
            var orderDetails = _dbContext.OrderDetails.Include(od => od.Order).ToList();

            return View(orderDetails);
        }

        public IActionResult GetTopProducts()
        {
            var orderDetails = _dbContext.OrderDetails.Include(od => od.Order).ToList();
            var topProducts = _dbContext.OrderDetails
                .GroupBy(od => od.Product)
                .Select(g => new { ProductName = g.Key.proName, Quantity = g.Sum(od => od.quantity) })
                .OrderByDescending(x => x.Quantity)
                .Take(3)
                .ToList();

            // Trả về dữ liệu dưới dạng JSON
            return Json(topProducts);
        }
        public IActionResult PaymentMethodCount()
        {
            var paymentMethodCounts = _dbContext.Orders
                .Where(o => o.paymentMethod != null) // Chỉ lấy những đơn hàng có phương thức thanh toán được xác định
                .GroupBy(o => o.paymentMethod)
                .Select(g => new { paymentMethod = g.Key, Count = g.Count() })
                .ToList();

            return Json(paymentMethodCounts);
        }
        public IActionResult GetOrderCountsCurrentMonth()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var dates = Enumerable.Range(0, (lastDayOfMonth - firstDayOfMonth).Days + 1)
                .Select(offset => firstDayOfMonth.AddDays(offset))
                .ToList();

            var orderCounts = dates.Select(date =>
                new
                {
                    Date = date,
                    Count = _dbContext.Orders
                        .Count(o => o.orderDate.Date == date && o.IsDelivered == true)
                }).ToList();

            return Json(orderCounts);
        }

    }
}
