using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return View();
        }
        public IActionResult Create(int id)
        {
            IEnumerable<Customer> customers = _dbContext.Customers.ToList();
            return View(customers);
        }
        public IActionResult Edit(int id)
        {
            IEnumerable<Customer> customers = _dbContext.Customers.ToList();
            return View(customers);
        }
    }
}
