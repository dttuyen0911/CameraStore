using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
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
        public IActionResult Create([FromForm] Feedback feedback, IFormFile feedImage, [FromForm] int StarRating, int orderId)
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

                feedback.StarRating = StarRating;

                _dbContext.Feedbacks.Add(feedback);
                _dbContext.SaveChanges();

                return Ok(new { success = true , feedImageUrl = feedback.feedUrlImage });
            }

            return View(feedback);
        }

        [HttpGet]
        public IActionResult CheckSubmit(int orderId)
        {
            OrderDetail orderDetail = _dbContext.OrderDetails
                .Include(od => od.Order)
                .FirstOrDefault(od => od.orderID == orderId);

            if (orderDetail != null && orderDetail.Order != null)
            {
                int customerID = orderDetail.Order.customerID;
                int proID = orderDetail.proID;

                // Truy vấn feedback dựa trên proId và customerId
                Feedback feedback = _dbContext.Feedbacks.FirstOrDefault(f => f.proID == proID && f.customerID == customerID);
                if (feedback != null)
                {
                    // Kiểm tra nếu có ảnh feedback
                    string imageUrl = null;
                    if (!string.IsNullOrEmpty(feedback.feedUrlImage))
                    {
                        imageUrl = Url.Content("~/Image/" + feedback.feedUrlImage); // Sử dụng Url.Content để tạo URL tương đối
                    }

                    // Nếu có feedback, trả về thông tin feedback
                    return Ok(new { success = true, feedback = feedback, feedImageUrl = imageUrl });
                }
                else
                {
                    // Nếu chưa có feedback, trả về thông báo
                    return Json(new { success = false });
                }
            }
            else
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy chi tiết đơn hàng
            }
        }
        [HttpGet]
        public IActionResult CalculateAverageRating(int proId)
        {
            var allFeedbacks = _dbContext.Feedbacks.Where(f => f.proID == proId).ToList();

            if (allFeedbacks.Count > 0)
            {
                int totalRatings = allFeedbacks.Sum(f => f.StarRating);
                double averageRating = (double)totalRatings / allFeedbacks.Count;
                int feedbackAccountCount = allFeedbacks.Select(f => f.customerID).Distinct().Count();

                var result = new
                {
                    AverageRating = averageRating,
                    FeedbackAccountCount = feedbackAccountCount
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

                var result = new
                {
                    AverageRating = averageRating,
                };

                return Json(result);
            }
            else
            {
                return Json(new { AverageRating = 0 });
            }
        }
        [HttpGet]
        public IActionResult CustomerFeedback(int proId)
        {
            var allFeedbacks = _dbContext.Feedbacks.Where(f => f.proID == proId).ToList();

            if (allFeedbacks.Count > 0)
            {
                int feedbackAccountCount = allFeedbacks.Select(f => f.customerID).Distinct().Count();

                var result = new
                {
                    FeedbackAccountCount = feedbackAccountCount
                };

                return Json(result);
            }
            else
            {
                return Json(new {FeedbackAccountCount = 0 });
            }
        }
       
    }
}
