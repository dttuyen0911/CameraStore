using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public SupplierController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Supplier> suppliers = _dbContext.Suppliers.ToList();
            return View(suppliers);
        }
        public IActionResult Create(int id)
        {
            IEnumerable<Supplier> suppliers = _dbContext.Suppliers.ToList();
            return View(suppliers);
        }
        public IActionResult Update(int id)
        {
            IEnumerable<Supplier> suppliers = _dbContext.Suppliers.ToList();
            return View(suppliers);
        }
    }
}
