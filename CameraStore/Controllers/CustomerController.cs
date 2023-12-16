using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CameraStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CustomerController(ApplicationDbContext dbContext)
        {
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
                    _dbContext.Customers.Add(obj);
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
                    // Detach the existing entity from the context
                    _dbContext.Entry(existingCustomer).State = EntityState.Detached;

                    // Set the key of the new object
                    obj.customerID = id;

                    // Attach and update the new object
                    _dbContext.Customers.Update(obj);

                    // Save changes
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
