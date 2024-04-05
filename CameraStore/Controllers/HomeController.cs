using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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
        [HttpPost]
        public IActionResult Search(string keyword)
        {
            IEnumerable<Product> products;

            if (string.IsNullOrEmpty(keyword))
            {
                // Nếu từ khóa là null hoặc rỗng, lấy tất cả sản phẩm
                products = _dbContext.Products
                    .Include(c => c.Category)
                    .Include(s => s.Supplier)
                    .ToList();
            }
            else
            {
                // Tìm kiếm sản phẩm theo từ khóa
                products = _dbContext.Products
                    .Include(c => c.Category)
                    .Include(s => s.Supplier)
                    .Where(p => p.proName.Contains(keyword))
                    .ToList();
            }

            // Kiểm tra nếu không có sản phẩm, trả về thông báo "No data"
            if (!products.Any())
            {
                return Content("No data");
            }
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
            ViewBag.MaxPrice = products.Max(p => p.proPrice);
            // Trả về một phần view chứa danh sách sản phẩm
            return PartialView("Store", products);
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
                return Ok();
            }
            // Cập nhật thông tin của khách hàng trong cơ sở dữ liệu
            existingCustomer.email = updatedCustomer.email;
            existingCustomer.fullname = updatedCustomer.fullname;
            existingCustomer.telephone = updatedCustomer.telephone;

            // Lưu thay đổi vào cơ sở dữ liệu
            _dbContext.SaveChanges();
            _notyf.Success("Information updated successfully.");
            return Ok();
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
                return Ok();// Trả về thông báo lỗi nếu mật khẩu hiện tại không đúng
            }

            existingCustomer.password = GetMD5(newPassword);
            _dbContext.SaveChanges();
            _notyf.Success("Password changed successfully.");
            return Ok();
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
        private List<Product> GetRecommendedProducts(int productId)
        {
            int userId = 0; // Sử dụng giá trị mặc định cho userId nếu người dùng không đăng nhập

            // Tiếp tục lấy lịch sử mua hàng của người dùng nếu có
            var userPurchases = _dbContext.Orders
                .Where(o => o.customerID == userId)
                .SelectMany(o => o.orderdetails.Select(od => od.proID))
                .ToList();

            // Tìm các người dùng khác đã mua các sản phẩm tương tự
            var similarUsers = _dbContext.Orders
                .Where(o => o.customerID != userId && o.orderdetails.Any(od => userPurchases.Contains(od.proID)))
                .Select(o => o.customerID)
                .Distinct()
                .ToList();

            // Tìm các sản phẩm được mua nhiều nhất bởi các người dùng tương tự
            var mostPurchasedProducts = _dbContext.Orders
                .Where(o => similarUsers.Contains(o.customerID))
                .SelectMany(o => o.orderdetails)
                .GroupBy(od => od.proID)
                .OrderByDescending(g => g.Count())
                .Take(9) // Điều chỉnh số lượng sản phẩm gợi ý tùy ý
                .Select(g => g.Key)
                .ToList();

            // Tìm sản phẩm được phản hồi tốt nhất
            var topRatedProducts = _dbContext.Products
                 .Where(p => p.Feedbacks.Any()) // Chỉ lấy sản phẩm có phản hồi
                 .OrderByDescending(p => p.Feedbacks.Average(f => f.StarRating)) // Sắp xếp theo đánh giá trung bình
                 .Take(5) // Lấy 5 sản phẩm phản hồi tốt nhất
                 .ToList();

            var topRatedProductIds = topRatedProducts.Select(p => p.proID).ToList();

            // Kết hợp danh sách các sản phẩm được đề xuất từ lịch sử mua hàng và sản phẩm được phản hồi tốt nhất
            var recommendedProducts = _dbContext.Products
                .Where(p => mostPurchasedProducts.Contains(p.proID) || topRatedProductIds.Contains(p.proID) && p.proID != productId) // Sử dụng Contains() để so sánh danh sách proID
                .Take(6 - topRatedProducts.Count) // Bổ sung thêm các sản phẩm được phản hồi tốt nhất để đảm bảo tổng số sản phẩm là 6
                .ToList();

            return recommendedProducts;
        }

    }
}
