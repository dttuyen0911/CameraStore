﻿using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CameraStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _logger = logger;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _dbContext.Products
                .Include(c => c.Category)
                .Include(s => s.Supplier)
                .ToList();
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
        [HttpPost]
        public IActionResult searchByName(string searchQuery)
        {
            IQueryable<Product> productsQuery = _dbContext.Products.Include(c => c.Category);

            // Kiểm tra xem searchQuery có tồn tại không
            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Tách các từ trong searchQuery và loại bỏ các khoảng trắng thừa
                var keywords = searchQuery.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Lọc các sản phẩm có tên chứa từ khóa nào đó trong danh sách từ khóa
                productsQuery = productsQuery.Where(p => keywords.Any(keyword => p.proName.Contains(keyword)));
            }

            var products = productsQuery.ToList();
            return PartialView("_ProductList", products);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
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
            return View();
        }
        public IActionResult AboutUs()
        {
            int userId = Convert.ToInt32(User.Identity.Name);

            var customer1 = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            if (customer1 != null)
            {
                ViewBag.FullName = customer1.fullname;
            }
            else
            {
                ViewBag.FullName = "Unknown";
            }
            var customer = _dbContext.Customers.ToList().AsEnumerable();
            var category = _dbContext.Categories.ToList().AsEnumerable();
            var supplier = _dbContext.Suppliers.ToList().AsEnumerable();
            var product = _dbContext.Products.ToList().AsEnumerable();

            return View((customer, category, supplier, product));
        }
        [Authorize]
        public IActionResult settingAccount()
        {
            int userId = Convert.ToInt32(User.Identity.Name);

            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            if (customer != null)
            {
                ViewBag.FullName = customer.fullname;
                return View(customer);
            }
            else
            {
                ViewBag.FullName = "Unknown";
                return View(new Customer());
            }
        }
        [HttpPost]
        [Authorize]
        public IActionResult ChangeInfo(Customer updatedCustomer)
        {
            // Lấy thông tin khách hàng hiện tại từ cơ sở dữ liệu
            Customer existingCustomer = _dbContext.Customers.Find(updatedCustomer.customerID);
            if (existingCustomer == null)
            {
                return NotFound();
            }
            if (updatedCustomer.email == null || updatedCustomer.fullname == null || updatedCustomer.telephone == null)
            {
                _notyf.Error("Please fill in all required fields in the Account.");
                return NoContent();
            }
            // Cập nhật thông tin của khách hàng trong cơ sở dữ liệu
            existingCustomer.email = updatedCustomer.email;
            existingCustomer.fullname = updatedCustomer.fullname;
            existingCustomer.telephone = updatedCustomer.telephone;

            // Lưu thay đổi vào cơ sở dữ liệu
            _dbContext.SaveChanges();
            _notyf.Success("Information updated successfully.");
            return NoContent();
        }

        private bool IsEmailUnique(string email, int? customerId = null)
        {
            var existingCustomer = _dbContext.Customers
                .FirstOrDefault(c => c.email == email && c.customerID != customerId);

            return existingCustomer == null;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword(int customerID, string currentPassword, string newPassword)
        {
            var existingCustomer = _dbContext.Customers.FirstOrDefault(c => c.customerID == customerID);

            if (existingCustomer == null)
            {
                return NotFound(); // Trả về kết quả không thành công nếu không tìm thấy khách hàng
            }

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                _notyf.Error("Please provide both current and new passwords.");
            }

            if (existingCustomer.password != GetMD5(currentPassword))
            {
                _notyf.Error("Incorrect current password or an error occurred.");
                return NoContent();// Trả về thông báo lỗi nếu mật khẩu hiện tại không đúng
            }
            if (!IsStrongPassword(newPassword))
            {
                _notyf.Error("New password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");
                return NoContent();
            }
            existingCustomer.password = GetMD5(newPassword);
            _dbContext.SaveChanges();
            _notyf.Success("Password changed successfully.");
            return NoContent();
        }
        private bool IsStrongPassword(string password)
        {
            // Độ dài ít nhất 8 ký tự và có ít nhất một chữ cái viết thường, một chữ cái viết hoa, một số, và một ký tự đặc biệt
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        public static string GetMD5(String str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromdata = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromdata);
            string byte25String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte25String += targetData[i].ToString("x2");
            }
            return byte25String;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult productDetail(int id)
        {
            int userId;
            if (User.Identity.IsAuthenticated)
            {
                userId = Convert.ToInt32(User.Identity.Name);
                var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
                if (customer != null)
                {
                    ViewBag.FullName = customer.fullname;
                }
                else
                {
                    ViewBag.FullName = "Unknown";
                }
            }
            else
            {
                ViewBag.FullName = "Unknown";
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = _dbContext.Products
                                    .Include(p => p.Feedbacks) // Nạp thông tin phản hồi của sản phẩm
                                    .ThenInclude(f => f.Customer) // Nạp thông tin của khách hàng cho mỗi phản hồi
                                    .FirstOrDefault(p => p.proID == id);

            if (product == null)
            {
                return NotFound();
            }

            var recommendedProductList = GetRecommendedProducts(product.proID);

            // Truyền cả sản phẩm hiện tại và danh sách sản phẩm được đề xuất vào view dưới dạng một đối tượng Tuple
            var model = new Tuple<Product, List<Product>>(product, recommendedProductList);

            return View(model);
        }
        public IActionResult Store(int page = 1, string priceRange = null)
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

            var products = productsQuery.ToList();

            int pageSize = 9;
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(paginatedProducts);
        }
        private List<Product> GetRecommendedProducts(int productId)
        {
            int userId = 0;

            var userPurchases = _dbContext.Orders
                 .Where(o => o.customerID == userId)
                 .SelectMany(o => o.orderdetails.Select(od => od.proID))
                 .Where(pId => pId != productId)
                 .ToList();

            var mostPurchasedProducts = _dbContext.Orders
                 .SelectMany(o => o.orderdetails)
                 .GroupBy(od => od.proID)
                 .OrderByDescending(g => g.Count())
                 .Select(g => g.Key)
                 .Where(pId => pId != productId) // Loại bỏ sản phẩm hiện đang xem
                 .ToList();


            mostPurchasedProducts.AddRange(userPurchases);

            mostPurchasedProducts = mostPurchasedProducts.Distinct().ToList();

            var topRatedProducts = _dbContext.Products
                .Where(p => p.Feedbacks.Any())
                .OrderByDescending(p => p.Feedbacks.Average(f => f.StarRating))
                .Take(5)
                .Where(p => p.proID != productId) 
                .ToList();

            var topRatedProductIds = topRatedProducts.Select(p => p.proID).ToList();

            var allRecommendedProducts = _dbContext.Products
                .Where(p => mostPurchasedProducts.Contains(p.proID) || topRatedProductIds.Contains(p.proID) && p.proID != productId)
                .ToList();

            var random = new Random();
            var recommendedProducts = allRecommendedProducts.OrderBy(x => random.Next()).Take(6).ToList();

            return recommendedProducts;
        }
        public IActionResult getFilterCategory(string[] selectedCategories)
        {
            IQueryable<Product> productsQuery = _dbContext.Products
                .Include(c => c.Category);

            // Check if selectedCategories is not null and contains items
            if (selectedCategories != null && selectedCategories.Length > 0)
            {
                productsQuery = productsQuery.Where(p => selectedCategories.Contains(p.Category.cateName));
            }

            var products = productsQuery.ToList();
            return PartialView("_ProductList", products);
        }
        public IActionResult getFilterStatus(string selectedStatus)
        {
            IQueryable<Product> productsQuery = _dbContext.Products
                .Include(c => c.Category);

            // Check if selectedStatus is not null or empty
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                productsQuery = productsQuery.Where(p => p.proStatus == selectedStatus);
            }

            var products = productsQuery.ToList();
            return PartialView("_ProductList", products);
        }

        public IActionResult sortByPrice(string sortByPrice = "Recommended")
        {
            IQueryable<Product> productsQuery = _dbContext.Products
                .Include(c => c.Category);

            switch (sortByPrice)
            {
                case "LowToHigh":
                    productsQuery = productsQuery.OrderBy(p => p.proSale != null ? p.proSale : p.proPrice);
                    break;
                case "HighToLow":
                    productsQuery = productsQuery.OrderByDescending(p => p.proSale != null ? p.proSale : p.proPrice);
                    break;
                default:
                    break;
            }

            var products = productsQuery.ToList();
            return PartialView("_ProductList", products);
        }

    }
}
