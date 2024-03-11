using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CameraStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _dbContext.Categories;
            IEnumerable<Product> products = _dbContext.Products.ToList();
            ViewBag.Categories = categories;
            return View(products);
        }

        public IActionResult Create()
        {
            ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
            ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
            /*if (ModelState.IsValid)
            {*/
            string fileName = proUploadImage(obj);
            obj.proUrlImage = fileName;

            _dbContext.Products.Add(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
            //}
            ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
            ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
            return View(obj);
        }
        public string proUploadImage(Product obj)
        {
            string uniqueFileName = null;
            if (obj.proImage != null)
            {
                string uploadsFoder = Path.Combine("wwwroot", "image");
                uniqueFileName = Guid.NewGuid().ToString() + obj.proID + obj.proImage.FileName;
                string filePath = Path.Combine(uploadsFoder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    obj.proImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult Edit(int id, string img)
        {
            ViewData["supID"] = new SelectList(_dbContext.Suppliers, "supID", "supName");
            ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
            Product obj = _dbContext.Products.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(int id, Product obj, string img)
        {
            /*if (ModelState.IsValid)
            {*/
            if (obj.proImage == null)
            {
                obj.proID = id;
                obj.proUrlImage = img;
                _dbContext.Products.Update(obj);
                _dbContext.SaveChanges();
            }
            else
            {
                obj.proID = id;
                string uniqueFileName = proUploadImage(obj);
                obj.proUrlImage = uniqueFileName;
                _dbContext.Products.Update(obj);
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
            //}
            ViewData["supID"] = new SelectList(_dbContext.Suppliers, "supID", "supName");
            ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
            return View(obj);
        }
        public IActionResult Delete(int id, string img)
        {
            Product obj = _dbContext.Products.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }

            if (obj.proUrlImage == null)
            {
                _dbContext.Products.Remove(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(img))
            {
                img = Path.Combine("wwwroot", "image", img);
                FileInfo infor = new FileInfo(img);

                if (infor.Exists)
                {
                    System.IO.File.Delete(img);
                    infor.Delete();
                }
            }

            _dbContext.Products.Remove(obj);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult detailPro(int? id, string img)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}