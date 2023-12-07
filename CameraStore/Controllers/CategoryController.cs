using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                string fileName = cateUploadImage(obj);
                obj.cateUrlImage = fileName;

                _dbContext.Categories.Add(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public string cateUploadImage(Category obj)
        {
            string uniqueFileName = null;
            if (obj.cateImage != null)
            {
                string uploadsFoder = Path.Combine("wwwroot", "image");
                uniqueFileName = Guid.NewGuid().ToString() + obj.cateID + obj.cateImage.FileName;
                string filePath = Path.Combine(uploadsFoder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    obj.cateImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult Update(int id, string img)
        {
            Category obj = _dbContext.Categories.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
                IEnumerable<Category> categories = _dbContext.Categories.ToList();
            return View(obj);
        }
        [HttpPost]
        public IActionResult Update(int id, Category obj, string img)
        {
            if (ModelState.IsValid)
            {
                if (obj.cateImage == null)
                {
                    obj.cateID = id;
                    obj.cateUrlImage = img;
                    _dbContext.Categories.Update(obj);
                    _dbContext.SaveChanges();
                }
                else
                {
                    obj.cateID = id;
                    string uniqueFileName = cateUploadImage(obj);
                    obj.cateUrlImage = uniqueFileName;
                    _dbContext.Categories.Update(obj);
                    _dbContext.SaveChanges();
                    img = Path.Combine("wwwroot", "image", img);
                    FileInfo infor = new FileInfo(img);
                    if (infor != null)
                    {
                        System.IO.File.Delete(img);
                        infor.Delete();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int id, string img)
        {
            Category obj = _dbContext.Categories.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (obj.cateUrlImage != null)
                {
                    img = Path.Combine("wwwroot", "uploads", img);
                    FileInfo infor = new FileInfo(img);
                    if (infor != null)
                    {
                        System.IO.File.Delete(img);
                        infor.Delete();
                    }
                }

                _dbContext.Categories.Remove(obj);
                _dbContext.SaveChanges();


                return RedirectToAction("Index");
            }
        }

    }
}
