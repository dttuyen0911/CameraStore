using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthenticationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
                    // Nếu không tìm thấy vai trò "Member" trong cơ sở dữ liệu, xử lý lỗi tương ứng
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
            var customer = _dbContext.Customers.FirstOrDefault(c => c.email == email);
            if (customer == null)
            {
                ModelState.AddModelError("email", "Email invalid.");
                return View();
            }

            if (customer.password != password)
            {
                ModelState.AddModelError("password", "Password invalid.");
                return View();
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, customer.fullname),
        new Claim(ClaimTypes.Email, customer.email),
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20) // Adjust expiration time as needed
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties).GetAwaiter().GetResult();

            var fullNameParts = customer.fullname.Split(' ');
            var lastName = fullNameParts[fullNameParts.Length - 1]; 

            TempData["WelcomeMessage"] = $"Welcome, {lastName}!";

            return RedirectToAction("Index", "Home"); // Redirect to desired page after successful login
        }



        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
            return RedirectToAction("Index", "Home"); // Redirect to desired page after logout
        }
    }
}
