using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Customer = CameraStore.Models.Customer;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CameraStore.Controllers
{

    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotyfService _notyf;
        public AuthenticationController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Terms()
        {
            return View();
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
                var existingEmail = _dbContext.Customers.Any(c => c.email == customer.email);
                var existingPhone = _dbContext.Customers.Any(c => c.telephone == customer.telephone);

                if (existingEmail)
                {
                    _notyf.Error("Email already exists, please enter another email");
                }
                else if (existingPhone)
                {
                    _notyf.Error("Telephone number already exists, please enter another telephone number");
                }
                else if (memberRole != null)
                {
                    Random rnd = new Random();
                    string otp = rnd.Next(100000, 999999).ToString();

                    // Gửi mã OTP qua email
                    SendOTPByEmail(customer.email, otp, customer.fullname);

                    // Lưu thông tin vào Session
                    HttpContext.Session.SetString("OTP", otp);
                    HttpContext.Session.SetString("Customer", JsonConvert.SerializeObject(customer));
                    return RedirectToAction("VerifyOTP");
                }
                else
                {
                    ModelState.AddModelError("", "Default role 'Member' not found.");
                }
            }
            return View(customer);
        }
        private string GenerateOTP()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }
        [HttpPost]
        public IActionResult ResendOTP()
        {
            if (HttpContext.Session.Keys.Contains("Customer"))
            {
                var customerJson = HttpContext.Session.GetString("Customer");
                var customer = JsonConvert.DeserializeObject<Customer>(customerJson);
                var email = customer.email;

                // Gửi lại mã OTP qua email
                // Đây là nơi gửi lại OTP mới mà không cần kiểm tra giá trị cũ
                var otp = GenerateOTP();
                SendOTPByEmail(email, otp, customer.fullname);

                _notyf.Success("OTP sent again successfully.");
                HttpContext.Session.SetString("OTP", otp);
                return Json(new { success = true, message = "OTP sent again successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Customer data not found in session." });
            }
        }

        public IActionResult VerifyOTP()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyOTP(string otp)
        {
            if (HttpContext.Session.Keys.Contains("OTP") && HttpContext.Session.Keys.Contains("Customer"))
            {
                string sessionOTP = HttpContext.Session.GetString("OTP");
                var customerJson = HttpContext.Session.GetString("Customer");
                var customer = JsonConvert.DeserializeObject<Customer>(customerJson);


                if (otp == sessionOTP)
                {
                    // Mã OTP hợp lệ, lưu thông tin người dùng vào CSDL
                    var memberRole = _dbContext.Roles.FirstOrDefault(r => r.name == "Member");
                    if (memberRole != null)
                    {
                        customer.password = GetMD5(customer.password);
                        customer.roleID = memberRole.roleID;
                        _dbContext.Customers.Add(customer);
                        _dbContext.SaveChanges();
                        _notyf.Success("Register successfully.");

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Default role 'Member' not found.");
                    }
                }
                else
                {
                    _notyf.Error("Invalid OTP, please enter OTP again");
                }

                // Nếu mã OTP không hợp lệ, hiển thị lại trang đăng ký để hiển thị popup
                return View("VerifyOTP", customer);
            }
            else
            {
                // TempData không chứa giá trị cần thiết, xử lý ở đây
                return View("VerifyOTP");
            }
        }
        public IActionResult VerifyOTPForgot()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyOTPForgot(string otp)
        {
            if (HttpContext.Session.Keys.Contains("OTP") && HttpContext.Session.Keys.Contains("Customer"))
            {
                string sessionOTP = HttpContext.Session.GetString("OTP");
                var customerJson = HttpContext.Session.GetString("Customer");
                var customer = JsonConvert.DeserializeObject<Customer>(customerJson);

                if (otp == sessionOTP)
                {
                    _notyf.Success("Verify OTP successfully.");

                    return RedirectToAction("ForgotPassword2");
                }
                else
                {
                    _notyf.Error("Invalid OTP, please enter OTP again");
                }

                // Nếu mã OTP không hợp lệ, hiển thị lại trang đăng ký để hiển thị popup
                return View("VerifyOTPForgot", customer);
            }
            _notyf.Error("Customer data not found. Please try again.");
            return View("ForgotPassword");
        }

        private void SendOTPByEmail(string email, string otp, string fullname)
        {
            try
            {
                // Tạo đối tượng MailMessage
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("tuyendtgcc200226@fpt.edu.vn"); // Địa chỉ email của bạn
                mail.To.Add(email); // Địa chỉ email của người nhận
                mail.Subject = "CAMERA DIGITAL STORE"; // Tiêu đề email
                mail.Body = $"Dear {fullname}, This is your authentication code, please do not share it with anyone. Your authentication code is: {otp}"; // Nội dung email

                // Cấu hình SMTP client
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); // SMTP server và cổng của Gmail
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("tuyendtgcc200226@fpt.edu.vn", "sscjzesgdqvbwjaf"); // Thay thế bằng email và mật khẩu của bạn

                // Gửi email
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email
                Console.WriteLine("Failed to send email. Error: " + ex.Message);
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Error()
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
                    if (customer.password != GetMD5(password))
                    {
                        ModelState.AddModelError("password", "Password invalid.");
                        return View();
                    }

                    // Lấy thông tin vai trò của khách hàng từ cơ sở dữ liệu
                    var role = _dbContext.Roles.FirstOrDefault(r => r.roleID == customer.roleID);
                    if (role == null)
                    {
                        ModelState.AddModelError("", "Role not found.");
                        return View();
                    }

                    // Set authentication cookie với các claims bao gồm vai trò của người dùng
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, customer.customerID.ToString()), // You can use any unique identifier for the user
                        new Claim(ClaimTypes.Email, customer.email), // If needed, you can include additional claims
                        new Claim(ClaimTypes.Role, role.name) // Add role claim
                    };

                    var identity = new ClaimsIdentity(claims, "ApplicationCookie");
                    var principal = new ClaimsPrincipal(identity);
                    var authProperties = new AuthenticationProperties
                    {
                        // Customize cookie properties if needed
                    };

                    HttpContext.SignInAsync("Cookies", principal, authProperties).Wait(); // Sign in the user
                    _notyf.Success("Login successfully.");

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
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var customer = _dbContext.Customers.FirstOrDefault(c => c.email == email);
            if (customer != null)
            {
                var otp = GenerateOTP();
                // Gửi mã OTP qua email
                SendOTPByEmail(customer.email, otp, customer.fullname);
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("Customer", JsonConvert.SerializeObject(customer));
                return RedirectToAction("VerifyOTPForgot");
            }
            else
            {
                _notyf.Warning("Email does not exist. Please enter another email.");
                return View();
            }
        }


        public IActionResult ForgotPassword2()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword2(string newPassword, string confirmPassword)
        {
            if (HttpContext.Session.Keys.Contains("Customer"))
            {
                var customerJson = HttpContext.Session.GetString("Customer");
                var customer = JsonConvert.DeserializeObject<Customer>(customerJson);

                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    _notyf.Error("Please provide both new and current passwords.");
                    return View(); // Giữ người dùng ở trang hiện tại khi có lỗi
                }
                if (!IsStrongPassword(newPassword))
                {
                    _notyf.Error("New password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");
                    return View();
                }
                if (newPassword != confirmPassword)
                {
                    _notyf.Error("New password and confirm password do not match.");
                    return View(); // Giữ người dùng ở trang hiện tại khi có lỗi
                }
                else
                {
                    customer.password = GetMD5(newPassword);
                    _dbContext.SaveChanges();
                    _notyf.Success("Password changed successfully.");
                }
                return RedirectToAction("Login");
                }
            _notyf.Error("Customer data not found. Please try again.");
            return View("ForgotPassword");
        }
        private bool IsStrongPassword(string password)
        {
            // Độ dài ít nhất 8 ký tự và có ít nhất một chữ cái viết thường, một chữ cái viết hoa, một số, và một ký tự đặc biệt
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }
    }
}
