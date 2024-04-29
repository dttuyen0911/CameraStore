using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;

        public RoleController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
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
                _notyf.Success("Create role successfully");
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
            
                obj.roleID = id;
                _dbContext.Roles.Update(obj);
                _dbContext.SaveChanges();
                _notyf.Success("Edit role successfully");
                return RedirectToAction("Index");
           
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
            _notyf.Success("Delete role successfully");
            return RedirectToAction("Index");
        }
        public IActionResult detailRole(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = _dbContext.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }
    }
}
