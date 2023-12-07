﻿using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

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
			IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
		public IActionResult productDetail(int id)
		{
			IEnumerable<Product> products = _dbContext.Products.ToList();
			return View(products);
		}
        public IActionResult Store(int id)
        {
            IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
            ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
            return View();
        }
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
                string fileName = proUploadImage(obj);
                obj.proUrlImage = fileName;

                _dbContext.Products.Add(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["supID"] = new SelectList(_dbContext.Suppliers.ToList(), "supID", "supName");
            ViewData["cateID"] = new SelectList(_dbContext.Categories.ToList(), "cateID", "cateName");
            return View(obj);
        }
        public string proUploadImage(Product obj)
        {
            string uniqueFileName = null;
            if (obj.proImage != null)
            {
                string uploadsFoder = Path.Combine("wwwroot", "Image");
                uniqueFileName = Guid.NewGuid().ToString() + obj.proID + obj.proImage.FileName;
                string filePath = Path.Combine(uploadsFoder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    obj.proImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult Update(int id)
        {
            IEnumerable<Product> products = _dbContext.Products.ToList();
            return View(products);
        }
    }
}
