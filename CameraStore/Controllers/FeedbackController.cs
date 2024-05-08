using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;

namespace CameraStore.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public FeedbackController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
        }
        [Authorize(Policy = "OwnerOrEmployeePolicy")]

        public IActionResult Index()
        {
            IEnumerable<Feedback> feedbacks = _dbContext.Feedbacks.ToList();
            return View(feedbacks);
        }
        
        public IActionResult Create(int orderID)
        {
            OrderDetail orderDetail = _dbContext.OrderDetails.FirstOrDefault(od => od.orderID == orderID);
            if (orderDetail == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy chi tiết đơn hàng
            }

            Feedback feedback = new Feedback
            {
                customerID = orderDetail.Order.customerID,
                proID = orderDetail.proID
            };

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([FromForm] Feedback feedback, IFormFile? feedImage, [FromForm] int StarRating, int orderId)
        {
            if (ModelState.IsValid)
            {
                OrderDetail orderDetail = _dbContext.OrderDetails
                    .Include(od => od.Order)
                    .FirstOrDefault(od => od.orderID == orderId);

                if (orderDetail != null && orderDetail.Order != null)
                {
                    feedback.customerID = orderDetail.Order.customerID;
                    feedback.proID = orderDetail.proID;
                }
                else
                {
                    return NotFound();
                }

                if (feedImage != null)
                {
                    string uploadsFolder = Path.Combine("wwwroot", "image");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(feedImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        feedImage.CopyTo(fileStream);
                    }
                    feedback.feedUrlImage = uniqueFileName;
                }
                else
                {
                    feedback.feedUrlImage = null; // Không có ảnh được tải lên, đặt feedUrlImage thành null
                }
                feedback.StarRating = StarRating;

                _dbContext.Feedbacks.Add(feedback);
                _dbContext.SaveChanges();
                _notyf.Success("Feedback successfully");
                return RedirectToAction("orderDetail", "OrderDetail", new { id = orderId });
            }

            return View(feedback);
        }
        [HttpGet]
        [Authorize]
        public IActionResult CheckSubmit(int orderId)
        {
            OrderDetail orderDetail = _dbContext.OrderDetails
                .Include(od => od.Order)
                .FirstOrDefault(od => od.orderID == orderId);

            if (orderDetail != null && orderDetail.Order != null)
            {
                int customerID = orderDetail.Order.customerID;
                int proID = orderDetail.proID;
                Feedback feedback = _dbContext.Feedbacks
                    .FirstOrDefault(f => f.proID == proID && f.customerID == customerID && f.orderID == orderId);
                if (feedback != null)
                {
                    string imageUrl = null;
                    if (!string.IsNullOrEmpty(feedback.feedUrlImage))
                    {
                        imageUrl = Url.Content("~/Image/" + feedback.feedUrlImage);
                    }

                    return Json(new { success = true, feedback = feedback, feedImageUrl = imageUrl });
                }
            }

            // Nếu không tìm thấy feedback, trả về false
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult CalculateAverageRating(int proId)
        {
            var allFeedbacks = _dbContext.Feedbacks.Where(f => f.proID == proId).ToList();

            if (allFeedbacks.Count > 0)
            {
                int totalRatings = allFeedbacks.Sum(f => f.StarRating);
                double averageRating = (double)totalRatings / allFeedbacks.Count;
                var feedbackCount = _dbContext.Feedbacks
                                .Where(f => f.proID == proId)
                                .Count();
                var result = new
                {
                    AverageRating = averageRating,
                    FeedbackAccountCount = feedbackCount
                };

                return Json(result);
            }
            else
            {
                return Json(new { AverageRating = 0, FeedbackAccountCount = 0 });
            }
        }
        [HttpGet]
        public IActionResult Calc(int proId)
        {
            var allFeedbacks = _dbContext.Feedbacks.Where(f => f.proID == proId).ToList();

            if (allFeedbacks.Count > 0)
            {
                int totalRatings = allFeedbacks.Sum(f => f.StarRating);
                double averageRating = (double)totalRatings / allFeedbacks.Count;

                double roundedAverageRating = Math.Round(averageRating, 1);

                var result = new
                {
                    AverageRating = roundedAverageRating,
                };

                return Json(result);
            }
            else
            {
                return Json(new { AverageRating = 0 });
            }
        }

        [HttpGet]
        public IActionResult countFeedback(int proId)
        {
            var feedbackCount = _dbContext.Feedbacks
                .Where(f => f.proID == proId)
                .Count();

            var result = new
            {
                FeedbackCount = feedbackCount
            };

            return Json(result);
        }
    }
}
