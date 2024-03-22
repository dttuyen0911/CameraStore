using CameraStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CameraStore.Controllers
{
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ManageController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult IndexOwner()
        {
            return View();
        }
        public IActionResult DashboardAdmin()
        {
            var customers = _dbContext.Customers.ToList().AsEnumerable();
            var roles = _dbContext.Roles.ToList().AsEnumerable();
            return View((customers, roles));
        }
        public IActionResult DashboardEmployee()
        {
            var product = _dbContext.Products.ToList().AsEnumerable();
            var category = _dbContext.Categories.ToList().AsEnumerable();
            var supplier = _dbContext.Suppliers.ToList().AsEnumerable();
            var cart = _dbContext.Carts.ToList().AsEnumerable();
            var order = _dbContext.Orders.ToList().AsEnumerable();
            var feedback = _dbContext.Feedbacks.ToList().AsEnumerable();
            return View((product, category,supplier,cart,order,feedback));
        }
        public IActionResult DashboardOwner()
        {
            var product = _dbContext.Products.ToList().AsEnumerable();
            var category = _dbContext.Categories.ToList().AsEnumerable();
            var supplier = _dbContext.Suppliers.ToList().AsEnumerable();
            var cart = _dbContext.Carts.ToList().AsEnumerable();
            var order = _dbContext.Orders.ToList().AsEnumerable();
            var feedback = _dbContext.Feedbacks.ToList().AsEnumerable();
            //statistical
            return View((product, category, supplier, cart, order, feedback));
        }
    }
}
