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

        public FeedbackController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
        public IActionResult Create(Feedback feedback, IFormFile feedImage, int rating, int orderId)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin chi tiết đơn hàng từ database dựa trên orderId
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

                // Xử lý file ảnh nếu có
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

                // Lưu giá trị rating vào feedback
                feedback.StarRating = rating;
                // Thêm feedback vào database và lưu thay đổi
                _dbContext.Feedbacks.Add(feedback);
                _dbContext.SaveChanges();

                // Chuyển hướng đến trang chính
                return Json(new { success = true });
            }

            // Trả về view feedback nếu ModelState không hợp lệ
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
                        imageUrl = "~/Image/" + feedback.feedUrlImage;
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
                return Json(new { success = false });
            }

        }

    }
}
