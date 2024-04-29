using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CameraStore.Controllers
{
    [Authorize(Policy = "AdminPolicy")]

    public class CustomerController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _dbContext;
        public CustomerController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _notyf = notyf;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Customer> customers = _dbContext.Customers.ToList();

            return View(customers);
        }
        public IActionResult Create()
        {
            ViewData["roleID"] = new SelectList(_dbContext.Roles.ToList(), "roleID", "name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Customer obj)
        {
            if (ModelState.IsValid)
            {
                if (IsEmailUnique(obj.email))
                {
                    obj.password = GetMD5(obj.password);

                    // get role member from database
                    var memberRole = _dbContext.Roles.FirstOrDefault(r => r.name == "Member");

                    if (memberRole != null)
                    {
                        // set role is member
                        obj.roleID = memberRole.roleID; 

                        _dbContext.Customers.Add(obj);
                        _dbContext.SaveChanges();
                        _notyf.Success("Creat account successfully.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // if not find role member is error
                        ModelState.AddModelError("", "Default role 'Member' not found.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }
            }

            ViewData["roleID"] = new SelectList(_dbContext.Roles.ToList(), "roleID", "name");
            return View(obj);
        }

        private bool IsEmailUnique(string email, int? customerId = null)
        {
            var existingCustomer = _dbContext.Customers
                .FirstOrDefault(c => c.email == email && c.customerID != customerId);

            return existingCustomer == null;
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
       
        public IActionResult Edit(int id)
        {
            ViewData["roleID"] = new SelectList(_dbContext.Roles.ToList(), "roleID", "name");
            Customer obj = _dbContext.Customers.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(int id, Customer obj)
        {
            Customer existingCustomer = _dbContext.Customers.Find(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (IsEmailUnique(obj.email, id))
                {
                    // Nếu mật khẩu không thay đổi, không cần mã hóa lại
                    if (obj.password != existingCustomer.password)
                    {
                        // Mã hóa mật khẩu mới trước khi cập nhật
                        obj.password = GetMD5(obj.password);
                    }

                    // Cập nhật thông tin của khách hàng trong cơ sở dữ liệu
                    existingCustomer.email = obj.email;
                    existingCustomer.fullname = obj.fullname;
                    existingCustomer.telephone = obj.telephone; // Mật khẩu đã được mã hóa (nếu cần)
                    existingCustomer.password = obj.password; // Mật khẩu đã được mã hóa (nếu cần)
                    existingCustomer.roleID = obj.roleID;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _dbContext.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }
            }

            ViewData["roleID"] = new SelectList(_dbContext.Roles.ToList(), "roleID", "name");
            return View(obj);
        }


        public ActionResult Delete(int id)
        {
            var obj = _dbContext.Customers.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _dbContext.Customers.Remove(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult detailCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _dbContext.Customers.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
