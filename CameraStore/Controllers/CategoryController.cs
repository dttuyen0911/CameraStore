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
        public IActionResult Create(Category cate)
        {
            if (ModelState.IsValid)
            {
                string fileName = cateUploadImage(cate);
                cate.cateUrlImage = fileName;

                _dbContext.Categories.Add(cate);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cate);
        }
        public string cateUploadImage(Category cate)
        {
            string uniqueFileName = null;
            if (cate.cateImage != null)
            {
                string uploadsFoder = Path.Combine("wwwroot", "image");
                uniqueFileName = Guid.NewGuid().ToString() + cate.cateID + cate.cateImage.FileName;
                string filePath = Path.Combine(uploadsFoder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    cate.cateImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult Edit(int id, string img)
        {
            Category cate = _dbContext.Categories.Find(id);
            if (cate == null)
            {
                return RedirectToAction("Index");
            }
            return View(cate);
        }
        [HttpPost]
        public IActionResult Edit(int id, Category cate, string img)
        {
            if (ModelState.IsValid)
            {
                if (cate.cateImage == null)
                {
                    cate.cateID = id;
                    cate.cateUrlImage = img;
                    _dbContext.Categories.Update(cate);
                    _dbContext.SaveChanges();
                }
                else
                {
                    cate.cateID = id;
                    string uniqueFileName = cateUploadImage(cate);
                    cate.cateUrlImage = uniqueFileName;
                    _dbContext.Categories.Update(cate);
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
            return View(cate);
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
                if (obj.cateUrlImage == null)
                {
                    _dbContext.Categories.Remove(obj);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    img = Path.Combine("wwwroot", "image", img);
                    FileInfo infor = new FileInfo(img);
                    if (infor != null)
                    {
                        System.IO.File.Delete(img);
                        infor.Delete();
                    }
                    _dbContext.Categories.Remove(obj);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
        }
        public IActionResult detailCate(int? id, string img)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _dbContext.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
