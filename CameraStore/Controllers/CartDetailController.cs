﻿using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace CameraStore.Controllers
{
    public class CartDetailController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public CartDetailController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
        }
        [Authorize(Policy = "OwnerOrEmployeePolicy")]
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartDetails = _dbContext.CartDetails
                .Where(cd => cd.cartID == id)
                .Include(cd => cd.Product)
                .ToList();
            return View(cartDetails);
        }
        [Authorize]
        public IActionResult Cart()
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value; // Lấy ID của người dùng từ cookie

            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            int userId = Convert.ToInt32(User.Identity.Name);

            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            if (customer != null)
            {
                ViewBag.FullName = customer.fullname;
            }
            else
            {
                ViewBag.FullName = "Unknown";
            }
            var cart = _dbContext.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.customerID == int.Parse(customerId));

            if (cart == null || cart.CartDetails.Count == 0)
            {
                ViewBag.Message = "No items in cart";
                return View(); 
            }

            var cartDetails = cart.CartDetails.ToList(); 

            return View(cartDetails); 
        }
    }
}
