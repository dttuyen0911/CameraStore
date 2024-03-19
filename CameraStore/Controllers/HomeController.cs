using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;

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
            var allCategories = _dbContext.Categories
                    .ToList();
            ViewBag.AllCategories = allCategories;
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
                return NotFound();
            }

            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public IActionResult Store(string selectedStatus, string[] selectedCategories, string sortByPrice = "Recommended", int page = 1, int pageSize = 10)
        {
            var categoriesDict = new Dictionary<string, int>();
            var status = new Dictionary<string, int>();
            var allProducts = _dbContext.Products.ToList();
            var allCategories = _dbContext.Categories
                    .Select(c => c.cateName)
                    .Distinct()
                    .ToList();
            var allStatus = _dbContext.Products
                    .Select(p => p.proStatus)
                    .Distinct()
                    .ToList();
            ViewBag.AllStatus = allStatus;
            ViewBag.AllCategories = allCategories;
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
            IQueryable<Product> productsQuery = _dbContext.Products
                .Include(c => c.Category);
            if (selectedCategories != null && selectedCategories.Length > 0)
            {
                productsQuery = productsQuery.Where(p => selectedCategories.Contains(p.Category.cateName));
            }
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                productsQuery = productsQuery.Where(p => p.proStatus == selectedStatus);
            }
            switch (sortByPrice)
            {
                case "LowToHigh":
                    productsQuery = productsQuery.OrderBy(p => p.proPrice);
                    break;
                case "HighToLow":
                    productsQuery = productsQuery.OrderByDescending(p => p.proPrice);
                    break;
                default:
                    break;
            }

            var products = productsQuery.ToList();

            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.MaxPrice = products.Max(p => p.proPrice);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(paginatedProducts);
        }
    }
}
