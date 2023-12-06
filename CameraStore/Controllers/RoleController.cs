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
        public IActionResult Create(int id)
        {
            IEnumerable<Role> roles = _dbContext.Roles.ToList();
            return View(roles);
        }
        public IActionResult Update(int id)
        {
            IEnumerable<Role> roles = _dbContext.Roles.ToList();
            return View(roles);
        }
    }
}
