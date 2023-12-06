using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _dbContext.Categories.ToList();
            return View(categories);
        }
        public IActionResult Create(int id)
        {
            IEnumerable<Category> categories = _dbContext.Categories.ToList();
            return View(categories);
        }
        public IActionResult Update(int id)
        {
            IEnumerable<Category> categories = _dbContext.Categories.ToList();
            return View(categories);
        }
    }
}
