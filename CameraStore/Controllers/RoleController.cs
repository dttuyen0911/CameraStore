using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public RoleController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Role> roles = _dbContext.Roles.ToList();
            return View(roles);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Role obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Roles.Add(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Edit(int id)
        {
            Role obj = _dbContext.Roles.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }     
        [HttpPost]
        public IActionResult Edit(int id, Role obj)
        {
            if (ModelState.IsValid)
            {
                obj.roleID = id;
                _dbContext.Roles.Update(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public ActionResult Delete(int id)
        {
            var obj = _dbContext.Roles.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _dbContext.Roles.Remove(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
