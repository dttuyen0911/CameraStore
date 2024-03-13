using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing.Printing;

namespace CameraStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _dbContext.Products
                .Include(c => c.Category)
                .Include(s => s.Supplier)
                .ToList();
            return View(products); ;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult productDetail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public IActionResult Store(int page = 1, int pageSize = 10)
        {
            var categoriesDict = new Dictionary<string, int>();
            var status = new Dictionary<string, int>();

            IEnumerable<Product> products = _dbContext.Products
                .Include(c => c.Category)
                .ToList();

            foreach (var product in products)
            {
                string categoryName = product.Category.cateName;
                if (categoriesDict.ContainsKey(categoryName))
                {
                    categoriesDict[categoryName]++;
                }
                else
                {
                    categoriesDict[categoryName] = 1;
                }
            }
            foreach (var product in products)
            {
                string stt = product.proStatus;
                if (status.ContainsKey(stt))
                {
                    status[stt]++;
                }
                else
                {
                    status[stt] = 1;
                }
            }

            // Phân trang
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.Status = status;
            ViewBag.MaxPrice = products.Max(p => p.proPrice);
            ViewBag.CategoriesDict = categoriesDict;
            ViewBag.TotalProducts = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(paginatedProducts);
        }

    }
}
