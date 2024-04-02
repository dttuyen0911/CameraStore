using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace CameraStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public OrderController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            IEnumerable<Order> orders = _dbContext.Orders.ToList();
            // Kiểm tra xem User.Identity có null không trước khi truy cập Name
            int userId = Convert.ToInt32(User.Identity.Name); // Cần kiểm tra null ở đây
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            if (customer != null)
            {
                ViewBag.FullName = customer.fullname;
            }
            else
            {
                ViewBag.FullName = "Unknown";
            }
            return View(orders);
        }

        public IActionResult CreateOrder(string selectedProductIds)
        {
            // Kiểm tra xem User.Identity có null không trước khi truy cập Name
            int userId = Convert.ToInt32(User.Identity.Name); // Cần kiểm tra null ở đây
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            if (customer == null)
            {
                // Xử lý khi không tìm thấy thông tin khách hàng
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng của khách hàng từ database
            var cart = _dbContext.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.customerID == userId);

            // Kiểm tra xem cart có null không trước khi truy cập thuộc tính của nó
            if (cart == null || cart.CartDetails == null || cart.CartDetails.Count == 0)
            {
                // Xử lý khi không có sản phẩm trong giỏ hàng
                return RedirectToAction("Index", "Cart");
            }

            if (string.IsNullOrEmpty(selectedProductIds))
            {
                // Xử lý khi không có sản phẩm nào được chọn
                ViewBag.ErrorMessage = "Please choose product to order.";
                return View("Error");
            }

            int[] selectedProductIdsArray;
            try
            {
                selectedProductIdsArray = selectedProductIds.Split(',').Select(int.Parse).ToArray();
            }
            catch (FormatException)
            {
                // Xử lý khi chuỗi không thể chuyển đổi thành các số nguyên
                ViewBag.ErrorMessage = "Invalid product IDs.";
                return View("Error");
            }

            // Lọc ra các sản phẩm được chọn từ giỏ hàng của khách hàng
            var selectedProducts = cart.CartDetails.Where(cd => selectedProductIdsArray.Contains(cd.proID)).ToList();

            // Gửi thông tin giỏ hàng và thông tin khách hàng đến view
            ViewBag.FullName = customer.fullname;
            ViewBag.Telephone = customer.telephone;
            ViewBag.Cart = selectedProducts;
            ViewBag.SelectedProductIds = selectedProductIds;

            return View("createOrder");
        }

        [HttpPost]
        public IActionResult SubmitOrder(Order order, string paymentMethod, string selectedProductIds)
        {
            // Kiểm tra User.Identity có null không trước khi truy cập Name
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            // Lấy thông tin khách hàng từ database
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            // Kiểm tra customer có null không trước khi truy cập thuộc tính của nó
            if (customer == null)
            {
                // Xử lý khi không tìm thấy thông tin khách hàng
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng của khách hàng từ database
            var cart = _dbContext.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.customerID == userId);

            // Kiểm tra cart có null không trước khi truy cập thuộc tính của nó
            if (cart == null || cart.CartDetails.Count == 0)
            {
                // Xử lý khi không có sản phẩm trong giỏ hàng
                return RedirectToAction("Index", "Cart");
            }

            var deliveryDate = DateTime.Now.AddDays(3); // Ngày giao hàng là ngày đặt + 3 ngày

            // Kiểm tra xem đã chọn phương thức thanh toán chưa
            if (string.IsNullOrEmpty(paymentMethod))
            {
                ViewBag.Error = "Please select a payment method.";
                ViewBag.FullName = order.orderFullname;
                ViewBag.Telephone = order.orderPhone;
                ViewBag.Cart = cart;
                return View("CreateOrder", order);
            }

            // Kiểm tra xem các trường bắt buộc đã được điền đầy đủ chưa
            if (string.IsNullOrEmpty(order.orderAddress) || string.IsNullOrEmpty(order.orderFullname) || string.IsNullOrEmpty(order.orderPhone))
            {
                ViewBag.Error = "Please fill in all required fields.";
                ViewBag.FullName = order.orderFullname;
                ViewBag.Telephone = order.orderPhone;
                ViewBag.Cart = cart;
                return View("CreateOrder", order);
            }

            var selectedProductIdsArray = selectedProductIds.Split(',').Select(int.Parse).ToArray();
            var selectedProducts = cart.CartDetails.Where(cd => selectedProductIdsArray.Contains(cd.proID)).ToList();

            if (selectedProducts != null && selectedProducts.Any())
            {
                // Kiểm tra xem đã tồn tại order nào chứa selectedProducts chưa
                var existingOrder = _dbContext.Orders
                    .Include(o => o.orderdetails)
                    .FirstOrDefault(o => o.customerID == userId &&
                                         o.orderdetails.Any(od => selectedProductIdsArray.Contains(od.proID)));

                if (existingOrder != null)
                {
                    // Tạo đơn hàng mới với thông tin mới
                    var newOrder = new Order
                    {
                        totalAmount = selectedProducts.Sum(item => item.quantity * item.Product.proPrice),
                        orderDate = DateTime.Now,
                        orderDelivery = DateTime.Now.AddDays(3),
                        orderStatus = false,
                        IsShipped = false,
                        IsDelivered = false,
                        customerID = userId,
                        orderFullname = order.orderFullname,
                        orderPhone = order.orderPhone,
                        orderAddress = order.orderAddress,
                        paymentMethod = paymentMethod
                    };

                    // Thêm order mới vào DbContext
                    _dbContext.Orders.Add(newOrder);
                    _dbContext.SaveChanges();

                    // Tạo danh sách mới các order detail cho order mới
                    // Tạo danh sách mới các order detail cho order mới
                    var newOrderDetails = new List<OrderDetail>();
                    foreach (var item in selectedProducts)
                    {
                        var orderDetail = new OrderDetail
                        {
                            orderID = newOrder.orderID, // Gắn orderID của đơn hàng mới
                            proID = item.proID,
                            quantity = item.quantity,
                            unitPrice = item.Product.proPrice
                        };

                        // Tìm các OrderDetail đã tồn tại trong DbContext với cùng một khóa chính
                        var existingOrderDetail = _dbContext.OrderDetails
                            .FirstOrDefault(od => od.orderID == orderDetail.orderID && od.proID == orderDetail.proID);

                        // Nếu không tìm thấy OrderDetail nào trùng khóa chính, thêm vào danh sách mới
                        if (existingOrderDetail == null)
                        {
                            newOrderDetails.Add(orderDetail);
                        }
                    }

                    // Thêm các order detail mới vào DbContext và lưu thay đổi
                    _dbContext.OrderDetails.AddRange(newOrderDetails);
                    _dbContext.SaveChanges();

                    // Xóa các sản phẩm đã chọn từ giỏ hàng
                    foreach (var selectedProduct in selectedProducts)
                    {
                        var cartDetailToRemove = cart.CartDetails.FirstOrDefault(cd => cd.proID == selectedProduct.proID);
                        if (cartDetailToRemove != null)
                        {
                            _dbContext.CartDetails.Remove(cartDetailToRemove);
                        }
                    }
                    _dbContext.SaveChanges();

                    return RedirectToAction("Index", "Order");
                }
                else
                {
                    var newOrder = new Order
                    {
                        totalAmount = selectedProducts.Sum(item => item.quantity * item.Product.proPrice),
                        orderDate = DateTime.Now,
                        orderDelivery = deliveryDate,
                        orderStatus = false,// Chưa xác nhận đơn hàng
                        IsShipped = false,
                        IsDelivered = false,
                        customerID = userId,
                        orderAddress = order.orderAddress,
                        orderFullname = order.orderFullname,
                        orderPhone = order.orderPhone,
                        paymentMethod = paymentMethod
                    };

                    // Thêm đơn hàng mới vào DbContext
                    _dbContext.Orders.Add(newOrder);
                    _dbContext.SaveChanges();

                    // Tạo danh sách mới các order detail cho order mới
                    var newOrderDetails = new List<OrderDetail>();
                    foreach (var item in selectedProducts)
                    {
                        var orderDetail = new OrderDetail
                        {
                            orderID = newOrder.orderID, // Gắn orderID của đơn hàng mới
                            proID = item.proID,
                            quantity = item.quantity,
                            unitPrice = item.Product.proPrice
                        };

                        // Tìm các OrderDetail đã tồn tại trong DbContext với cùng một khóa chính
                        var existingOrderDetail = _dbContext.OrderDetails
                            .FirstOrDefault(od => od.orderID == orderDetail.orderID && od.proID == orderDetail.proID);

                        // Nếu không tìm thấy OrderDetail nào trùng khóa chính, thêm vào danh sách mới
                        if (existingOrderDetail == null)
                        {
                            newOrderDetails.Add(orderDetail);
                        }
                    }

                    // Thêm các order detail mới vào DbContext và lưu thay đổi
                    _dbContext.OrderDetails.AddRange(newOrderDetails);
                    _dbContext.SaveChanges();
                    foreach (var selectedProduct in selectedProducts)
                    {
                        var cartDetailToRemove = cart.CartDetails.FirstOrDefault(cd => cd.proID == selectedProduct.proID);
                        if (cartDetailToRemove != null)
                        {
                            _dbContext.CartDetails.Remove(cartDetailToRemove);
                        }
                    }
                    _dbContext.SaveChanges();

                    return RedirectToAction("viewOrder", "OrderDetail");
                }

            }
            return RedirectToAction("viewOrder", "OrderDetail");

        }
        public IActionResult Confirm(int ?orderId)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            // Lấy thông tin khách hàng từ database
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            // Kiểm tra customer có null không trước khi truy cập thuộc tính của nó
            if (customer == null)
            {
                // Xử lý khi không tìm thấy thông tin khách hàng
                return RedirectToAction("Index", "Home");
            }
            var order = _dbContext.Orders.FirstOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                // Xử lý khi không tìm thấy đơn hàng
                return RedirectToAction("Index", "Order");
            }

            order.orderStatus = true;

            // Lưu thay đổi vào DbContext
            _dbContext.SaveChanges();

            // Chuyển hướng người dùng đến trang Index của đơn hàng sau khi xác nhận
            return RedirectToAction("Index", "Order");
        }
        public IActionResult EnRoute(int? orderId)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            // Lấy thông tin khách hàng từ database
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            // Kiểm tra customer có null không trước khi truy cập thuộc tính của nó
            if (customer == null)
            {
                // Xử lý khi không tìm thấy thông tin khách hàng
                return RedirectToAction("Index", "Home");
            }
            var order = _dbContext.Orders.FirstOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                // Xử lý khi không tìm thấy đơn hàng
                return RedirectToAction("Index", "Order");
            }

            order.IsShipped = true;

            // Lưu thay đổi vào DbContext
            _dbContext.SaveChanges();

            // Chuyển hướng người dùng đến trang Index của đơn hàng sau khi xác nhận
            return RedirectToAction("Index", "Order");
        }
        public IActionResult Received(int? orderId)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            // Lấy thông tin khách hàng từ database
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            // Kiểm tra customer có null không trước khi truy cập thuộc tính của nó
            if (customer == null)
            {
                // Xử lý khi không tìm thấy thông tin khách hàng
                return RedirectToAction("Index", "Home");
            }

            var order = _dbContext.Orders.FirstOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                // Xử lý khi không tìm thấy đơn hàng
                return RedirectToAction("Index", "Order");
            }

            order.IsDelivered = true;
            _dbContext.SaveChanges();
            return Json(new { success = true });
        }

    }
}
