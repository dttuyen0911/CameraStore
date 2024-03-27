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
            return View();
        }
		public IActionResult GetTopProducts()
		{
			var topProducts = _dbContext.OrderDetails
				.GroupBy(od => od.proID)
				.Select(g => new { ProductId = g.Key, Quantity = g.Sum(od => od.quantity) })
				.OrderByDescending(x => x.Quantity)
				.Take(3)
				.ToList();

			// Chuyển đổi dữ liệu sang định dạng cần thiết
			var labels = topProducts.Select(p => p.ProductId.ToString()).ToList();
			var quantities = topProducts.Select(p => p.Quantity).ToList();

			// Trả về dữ liệu dưới dạng JSON
			return Json(new { labels = labels, quantities = quantities });
		}
	}
}
