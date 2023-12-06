using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CameraStore.Controllers
{
    public class ProductController : Controller
    {
		private readonly ApplicationDbContext _dbContext;
		public ProductController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public IActionResult Index()
        {
			IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
		public IActionResult productDetail(int id)
		{
			IEnumerable<Product> products = _dbContext.Products.ToList();
			return View(products);
		}
        public IActionResult Store(int id)
        {
            IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
        public IActionResult Create(int id)
        {
            IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
        public IActionResult Update(int id)
        {
            IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
    }
}
