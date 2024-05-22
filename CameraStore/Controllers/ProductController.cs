using AspNetCoreHero.ToastNotification.Abstractions;
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
    [Authorize(Policy = "OwnerOrEmployeePolicy")]

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public ProductController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
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
            if(obj.proPrice <= 0)
            {
                _notyf.Error("Price must be a positive number.");
                return View(obj);
            }
            if (obj.proPercent <= 0)
            {
                _notyf.Error("Percent must be a positive number.");
                return View(obj);
            }
            if (obj.supID == null || obj.cateID == null || !_dbContext.Suppliers.Any(s => s.supID == obj.supID) || !_dbContext.Categories.Any(c => c.cateID == obj.cateID))
            {
                _notyf.Error("Please select both category and supplier.");
                ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
                ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
                return View(obj);
            }

            string fileName = proUploadImage(obj);
            if (fileName == null)
            {
                _notyf.Error("Invalid image file.");
                ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
                ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
                return View(obj);
            }
            obj.proUrlImage = fileName;

            // Kiểm tra proPercent có giá trị hay không
            if (obj.proPercent != null)
            {
                // Nếu proPercent có giá trị, tính proSale dựa trên proPrice và proPercent
                obj.proSale = obj.proPrice * (100 - obj.proPercent) / 100;
            }
            else
            {
                // Nếu proPercent là null, proSale bằng proPrice
                obj.proSale = obj.proPrice;
            }
            _dbContext.Products.Add(obj);
            _dbContext.SaveChanges();
            _notyf.Success("Add product sucessfully");

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
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(obj.proImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return null; // Invalid file extension
                }

                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedMimeTypes.Contains(obj.proImage.ContentType))
                {
                    return null; // Invalid MIME type
                }

                string uploadsFolder = Path.Combine("wwwroot", "image");
                uniqueFileName = Guid.NewGuid().ToString() + obj.proID + fileExtension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
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

            if (obj.proPercent == null)
            {
                // Nếu proPercent là null, gán proSale = proPrice
                obj.proSale = obj.proPrice;
            }
            else
            {
                // Nếu proPercent có giá trị, tính proSale dựa trên proPrice và proPercent
                obj.proSale = obj.proPrice * (100 - obj.proPercent) / 100;
            }
            if (obj.proImage == null)
            {
                obj.proID = id;
                obj.proUrlImage = img;
                if (obj.supID == null || obj.cateID == null || !_dbContext.Suppliers.Any(s => s.supID == obj.supID) || !_dbContext.Categories.Any(c => c.cateID == obj.cateID))
                {
                    _notyf.Error("Please select both category and supplier.");
                    ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
                    ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
                    return View(obj);
                }
                if (obj.proPrice <= 0)
                {
                    _notyf.Error("Price must be a positive number.");
                    return View(obj);
                }
                if (obj.proPercent <= 0)
                {
                   _notyf.Error("Percent must be a positive number.");
                    return View(obj); 
                }
                if(obj.proQuantity <= 0)
                {
                    _notyf.Error("Quantity must be a positive number.");
                    return View(obj);
                }
                if (obj.proQuantitySold <= 0)
                {
                    _notyf.Error("Quantity sold must be a positive number.");
                    return View(obj);
                }
                if (obj.proQuantity == 0)
                {
                    obj.proStatus = "Sold out";
                }
                else
                {
                    if (obj.proPercent != null && obj.proPercent > 0)
                    {
                        obj.proStatus = "Sale";
                    }
                    else
                    {
                        obj.proStatus = "New";
                    }
                }
                _dbContext.Products.Update(obj);
                _notyf.Success("Edit product sucessfully");

                _dbContext.SaveChanges();
            }
            else
            {
                obj.proID = id;
                string fileName = proUploadImage(obj);
                if (fileName == null)
                {
                    _notyf.Error("Invalid image file.");
                    ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
                    ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
                    return View(obj);
                }
                obj.proUrlImage = fileName;
                if (obj.supID == null || obj.cateID == null || !_dbContext.Suppliers.Any(s => s.supID == obj.supID) || !_dbContext.Categories.Any(c => c.cateID == obj.cateID))
                {
                    _notyf.Error("Please select both category and supplier.");
                    ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
                    ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
                    return View(obj);
                }
                if (obj.proPrice <= 0)
                {
                    _notyf.Error("Price must be a positive number.");
                    return View(obj);
                }
                if (obj.proPercent <= 0)
                {
                    _notyf.Error("Percent must be a positive number.");
                    return View(obj);
                }
                if (obj.proQuantity == 0)
                {
                    obj.proStatus = "Sold out";
                }
                else
                {
                    if (obj.proPercent != null && obj.proPercent > 0)
                    {
                        obj.proStatus = "Sale";
                    }
                    else
                    {
                        obj.proStatus = "New";
                    }
                }
                _dbContext.Products.Update(obj);
                _notyf.Success("Edit product sucessfully");
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
                _notyf.Success("Delete product sucessfully");
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
            _notyf.Success("Delete product sucessfully");

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