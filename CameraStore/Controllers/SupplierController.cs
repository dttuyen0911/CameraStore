using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace CameraStore.Controllers
{
    [Authorize(Policy = "EmployeeOwner")]

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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Supplier sup)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Suppliers.Add(sup);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sup);
        }
        public IActionResult Edit(int id)
        {
            Supplier obj = _dbContext.Suppliers.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(int id, Supplier sup)
        {
            if (ModelState.IsValid)
            {
                sup.supID = id;
                _dbContext.Suppliers.Update(sup);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sup);
        }
        public ActionResult Delete(int id)
        {
            var obj = _dbContext.Suppliers.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _dbContext.Suppliers.Remove(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult detailSup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = _dbContext.Suppliers.Find(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }
    }
}
