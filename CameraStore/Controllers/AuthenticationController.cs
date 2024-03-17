using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var memberRole = _dbContext.Roles.FirstOrDefault(r => r.name == "Member");
                customer.createAt = DateTime.Now;
                if (memberRole != null)
                {
                    // Gán vai trò mặc định là "Member" cho tài khoản mới
                    customer.roleID = memberRole.roleID; // Đây là giả sử tên cột chứa ID của vai trò trong đối tượng Customer là RoleID, điều này có thể thay đổi tùy theo thiết kế cơ sở dữ liệu của bạn

                    _dbContext.Customers.Add(customer);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Default role 'Member' not found.");
                }
            }
            return View(customer);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var customer = _dbContext.Customers.FirstOrDefault(c => c.email == email);
                if (customer != null)
                {
                    if (customer.password != password)
                    {
                        ModelState.AddModelError("password", "Password invalid.");
                        return View();
                    }

                    // Set authentication cookie
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.customerID.ToString()), // You can use any unique identifier for the user
                new Claim(ClaimTypes.Email, customer.email) // If needed, you can include additional claims
                // Add more claims as needed
            };

                    var identity = new ClaimsIdentity(claims, "ApplicationCookie");
                    var principal = new ClaimsPrincipal(identity);
                    var authProperties = new AuthenticationProperties
                    {
                        // Customize cookie properties if needed
                    };

                    HttpContext.SignInAsync("Cookies", principal, authProperties).Wait(); // Sign in the user

                    return RedirectToAction("Index", "Home"); // Redirect after successful login
                }
                else
                {
                    ModelState.AddModelError("email", "Email invalid.");
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            if (HttpContext.Session.IsAvailable)
            {
                HttpContext.Session.Remove("CustomerId");
                HttpContext.Session.Remove("WelcomeMessage");
                HttpContext.Session.Clear(); // Xóa tất cả các dữ liệu phiên
                HttpContext.SignOutAsync("Cookies").Wait();

                // Xác nhận việc xóa dữ liệu phiên và chuyển hướng người dùng về trang chính
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Trường hợp không có phiên hoặc không khả dụng, chuyển hướng người dùng về trang chính
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
