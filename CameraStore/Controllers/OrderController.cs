using AspNetCoreHero.ToastNotification.Abstractions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web.Helpers;

namespace CameraStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public OrderController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;
        }
        [Authorize(Policy = "OwnerOrEmployeePolicy")]

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
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cart = _dbContext.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.customerID == userId);

            if (cart == null || cart.CartDetails.Count == 0)
            {
                return RedirectToAction("Cart", "CartDetail");
            }

            var deliveryDate = DateTime.Now.AddDays(3); 

            if (string.IsNullOrEmpty(paymentMethod))
            {
                ViewBag.Error = "Please select a payment method.";
                ViewBag.FullName = order.orderFullname;
                ViewBag.Telephone = order.orderPhone;
                ViewBag.Cart = cart;
                return View("CreateOrder", order);
            }
            if (string.IsNullOrEmpty(order.orderAddress) || string.IsNullOrEmpty(order.orderFullname) || string.IsNullOrEmpty(order.orderPhone))
            {
                ViewBag.Error = "Please fill in all required fields.";
                ViewBag.FullName = order.orderFullname;
                ViewBag.Telephone = order.orderPhone;
                ViewBag.Cart = cart;
                return View("CreateOrder", order);
            }
            if (paymentMethod == "Cod")
            {
                order.IsPayment = true;
            }
            else
            {
                order.IsPayment = false;
            }
            var selectedProductIdsArray = selectedProductIds.Split(',').Select(int.Parse).ToArray();
            var selectedProducts = cart.CartDetails.Where(cd => selectedProductIdsArray.Contains(cd.proID)).ToList();

            if (selectedProducts != null && selectedProducts.Any())
            {
                var existingOrder = _dbContext.Orders
                    .Include(o => o.orderdetails)
                    .FirstOrDefault(o => o.customerID == userId &&
                                         o.orderdetails.Any(od => selectedProductIdsArray.Contains(od.proID)));

                if (existingOrder != null)
                {
                    var newOrder = new Order
                    {
                        totalAmount = selectedProducts.Sum(item => item.quantity * item.Product.proPrice),
                        orderDate = DateTime.Now,
                        orderDelivery = DateTime.Now.AddDays(3),
                        orderStatus = false,
                        IsShipped = false,
                        IsDelivered = false,
                        customerID = userId,
                        IsPayment = order.IsPayment,
                        orderFullname = order.orderFullname,
                        orderPhone = order.orderPhone,
                        orderAddress = order.orderAddress,
                        paymentMethod = paymentMethod
                      
                    };

                    _dbContext.Orders.Add(newOrder);
                    _dbContext.SaveChanges();

                    var newOrderDetails = new List<OrderDetail>();
                    foreach (var item in selectedProducts)
                    {
                        var orderDetail = new OrderDetail
                        {
                            orderID = newOrder.orderID, 
                            proID = item.proID,
                            quantity = item.quantity,
                            unitPrice = item.Product.proPrice
                        };

                        var existingOrderDetail = _dbContext.OrderDetails
                            .FirstOrDefault(od => od.orderID == orderDetail.orderID && od.proID == orderDetail.proID);

                        if (existingOrderDetail == null)
                        {
                            newOrderDetails.Add(orderDetail);
                        }
                    }

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
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    var jsonOrder = JsonSerializer.Serialize(newOrder, options);
                    HttpContext.Session.SetString("OrderInfo", jsonOrder);
                    SendEmailOrderConfirmation(customer);
                    if (paymentMethod == "Card")
                    {
                        return RedirectToAction("StripePayment");
                    }
                    _notyf.Success("Order successfully");
                    return RedirectToAction("viewOrder", "OrderDetail");
                }
                else
                {
                    var newOrder = new Order
                    {
                        totalAmount = selectedProducts.Sum(item => item.quantity * item.Product.proPrice),
                        orderDate = DateTime.Now,
                        orderDelivery = deliveryDate,
                        orderStatus = false,
                        IsShipped = false,
                        IsDelivered = false,
                        IsPayment = order.IsPayment,
                        customerID = userId,
                        orderAddress = order.orderAddress,
                        orderFullname = order.orderFullname,
                        orderPhone = order.orderPhone,
                        paymentMethod = paymentMethod
                    };

                    _dbContext.Orders.Add(newOrder);
                    _dbContext.SaveChanges();

                    var newOrderDetails = new List<OrderDetail>();
                    foreach (var item in selectedProducts)
                    {
                        var orderDetail = new OrderDetail
                        {
                            orderID = newOrder.orderID, 
                            proID = item.proID,
                            quantity = item.quantity,
                            unitPrice = item.Product.proPrice
                        };

                        var existingOrderDetail = _dbContext.OrderDetails
                            .FirstOrDefault(od => od.orderID == orderDetail.orderID && od.proID == orderDetail.proID);

                        if (existingOrderDetail == null)
                        {
                            newOrderDetails.Add(orderDetail);
                        }
                    }

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
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    var jsonOrder = JsonSerializer.Serialize(newOrder, options);
                    HttpContext.Session.SetString("OrderInfo", jsonOrder);
                    SendEmailOrderConfirmation(customer);
                    if (paymentMethod == "Card")
                    {
                        return RedirectToAction("StripePayment");
                    }
                    _notyf.Success("Order successfully");
                    return RedirectToAction("viewOrder", "OrderDetail");
                }

            }
            return RedirectToAction("viewOrder", "OrderDetail");

        }
        [HttpPost]
        public IActionResult CancelOrder(int orderId, int proID)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            var order = _dbContext.Orders
                .Include(o => o.orderdetails)
                .FirstOrDefault(o => o.orderID == orderId && o.customerID == userId);

            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var orderDetail in order.orderdetails)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.proID == orderDetail.proID);
                if (product != null)
                {
                    product.proQuantity += orderDetail.quantity;
                    product.proQuantitySold -= orderDetail.quantity;
                }
            }

            var productDetailToRemove = order.orderdetails.FirstOrDefault(od => od.proID == proID);
            if (productDetailToRemove != null)
            {
                HttpContext.Session.SetString("DeletedProduct", productDetailToRemove.proID.ToString());
                _dbContext.OrderDetails.Remove(productDetailToRemove);
            }

            _dbContext.SaveChanges();
            var remainingOrderDetails = _dbContext.OrderDetails.Where(od => od.orderID == orderId).ToList();
            if (remainingOrderDetails.Count == 0)
            {
                _dbContext.Orders.Remove(order);
                _dbContext.SaveChanges();
            }
            SendEmailOrderCancellation(order.customerID);
            _notyf.Success("Cancel order successfully");
            return RedirectToAction("viewOrder", "OrderDetail");
        }

        [HttpPost]
        private void SendEmailOrderCancellation(int customerId)
        {
            try
            {
                var deletedProduct = HttpContext.Session.GetString("DeletedProduct");
                if (!string.IsNullOrEmpty(deletedProduct))
                {
                    int proID = int.Parse(deletedProduct);

                    var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == customerId);

                    if (customer == null)
                    {
                        // Xử lý khi không tìm thấy thông tin khách hàng
                        return;
                    }


                    var product = _dbContext.Products.FirstOrDefault(p => p.proID == proID);

                    if (product == null)
                    {
                        return;
                    }
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("tuyendtgcc200226@fpt.edu.vn"); 
                    mail.To.Add(customer.email);
                    mail.Subject = "Order Cancellation Confirmation"; 

                    StringBuilder body = new StringBuilder();
                    body.AppendLine($"Dear {customer.fullname},");
                    body.AppendLine("We have received your order cancellation request.");
                    body.AppendLine($"The product '{product.proName}' has been removed from your order.");
                    body.AppendLine("If you have any questions or concerns, please feel free to contact us.");
                    body.AppendLine("Thank you.");

                    mail.Body = body.ToString();
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); 
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("tuyendtgcc200226@fpt.edu.vn", "sscjzesgdqvbwjaf"); 

                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email
                Console.WriteLine("Failed to send email. Error: " + ex.Message);
            }
        }

        private void SendEmailOrderConfirmation(Models.Customer customer)
        {
            try
            {
                var jsonOrder = HttpContext.Session.GetString("OrderInfo");
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                var orderInfo = JsonSerializer.Deserialize<Order>(jsonOrder, options);
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("tuyendtgcc200226@fpt.edu.vn"); 
                mail.To.Add(customer.email); 
                mail.Subject = "Confirm successful order of Camera Digital Store products";

                StringBuilder body = new StringBuilder();
                body.AppendLine($"Dear {customer.fullname},");
                body.AppendLine("Thank you for using Camera Digital Store's services.");
                body.AppendLine($"Camera Digital Store confirms that you have successfully ordered our product at {orderInfo.orderDate}.");
                body.AppendLine("Here are your order details:");

                body.AppendLine($"Order ID: {orderInfo.orderID}");
                body.AppendLine($"Order Date: {orderInfo.orderDate}");
                body.AppendLine($"Estimated delivery date: {orderInfo.orderDelivery.ToString("dd/MM/yyyy")}");
                body.AppendLine($"Total Amount: {orderInfo.totalAmount}");
                body.AppendLine($"Delivery Address: {orderInfo.orderAddress}");
                body.AppendLine($"Payment Method: {orderInfo.paymentMethod}");

                body.AppendLine("Ordered Products:");
                foreach (var detail in orderInfo.orderdetails)
                {
                    body.AppendLine($"- {detail.Product.proName}: {detail.quantity} x {detail.unitPrice}");
                }
                body.AppendLine("Thank you for shopping with us!");

                mail.Body = body.ToString();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); 
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("tuyendtgcc200226@fpt.edu.vn", "sscjzesgdqvbwjaf"); 

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email. Error: " + ex.Message);
            }
        }
        public IActionResult Confirm(int ?orderId)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var order = _dbContext.Orders.FirstOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Order");
            }

            order.orderStatus = true;

            _dbContext.SaveChanges();
            _notyf.Success("Confirm order successfully");

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

            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var order = _dbContext.Orders.FirstOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Order");
            }

            order.IsShipped = true;

            _dbContext.SaveChanges();
            _notyf.Success("Confirm the order has been successfully delivered to the shipping unit.");
            return RedirectToAction("Index", "Order");
        }
        public IActionResult Received(int? orderId, int? proId)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = _dbContext.Orders.FirstOrDefault(o => o.orderID == orderId );
            if (order == null)
            {
                return RedirectToAction("Index", "Order");
            }
            order.IsDelivered = true;
            _dbContext.SaveChanges();
            return RedirectToAction("orderDetail", "OrderDetail", new { orderid = orderId, proid = proId });
        }
        public ActionResult StripePayment()
        {
            // Lấy thông tin đơn hàng từ session
            var jsonOrder = HttpContext.Session.GetString("OrderInfo");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var orderInfo = JsonSerializer.Deserialize<Order>(jsonOrder, options);
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            // Lấy thông tin khách hàng từ database
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);
            // Kiểm tra xem đơn hàng có tồn tại không
            if (orderInfo != null)
            {
                var domain = "https://localhost:7256/";
                var option = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"OrderDetail/viewOrder", // Truyền paymentMethod qua URL
                    CancelUrl = domain + $"OrderDetail/viewOrder",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    CustomerEmail = customer.email
                };

                foreach (var orderDetail in orderInfo.orderdetails)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(orderDetail.quantity * orderDetail.Product.proPrice * 100), // Đổi giá sang đơn vị cents (VD: $10.00 -> 1000 cents)
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = orderDetail.Product.proName.ToString(),
                            }
                        },
                        Quantity = orderDetail.quantity
                    };
                    option.LineItems.Add(sessionListItem);
                }

                var service = new SessionService();
                Session session = service.Create(option);
                Response.Headers.Add("Location", session.Url);
                _notyf.Success("Order successfully");
                orderInfo.IsPayment = true;
                _dbContext.SaveChanges();

                return new StatusCodeResult(303);
            }
            else
            {
                // Xử lý khi không tìm thấy thông tin đơn hàng trong session
                return RedirectToAction("Cart", "CartDetail");
            }
        }
        public ActionResult StripePayment1(int orderid)
        {
            var orderInfo = _dbContext.Orders
             .Include(o => o.orderdetails) // Load các chi tiết đơn hàng
             .ThenInclude(od => od.Product) // Load thông tin sản phẩm
             .FirstOrDefault(o => o.orderID == orderid);
            if (orderInfo == null)
            {
                // Xử lý khi không tìm thấy thông tin đơn hàng trong cơ sở dữ liệu
                return RedirectToAction("Cart", "CartDetail");
            }

            var customerId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = Convert.ToInt32(customerId);

            // Lấy thông tin khách hàng từ database
            var customer = _dbContext.Customers.FirstOrDefault(c => c.customerID == userId);

            var domain = "https://localhost:7256/";
            var option = new SessionCreateOptions
            {
                SuccessUrl = domain + $"OrderDetail/viewOrder", // Truyền paymentMethod qua URL
                CancelUrl = domain + $"OrderDetail/viewOrder",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = customer.email
            };

            foreach (var orderDetail in orderInfo.orderdetails)
            {
                if (orderDetail != null && orderDetail.Product != null && orderDetail.Product.proName != null)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(orderDetail.quantity * orderDetail.Product.proPrice * 100), // Đổi giá sang đơn vị cents (VD: $10.00 -> 1000 cents)
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = orderDetail.Product.proName.ToString(),
                            }
                        },
                        Quantity = orderDetail.quantity
                    };
                    option.LineItems.Add(sessionListItem);
                }
            }

            var service = new SessionService();
            Session session = service.Create(option);
            Response.Headers.Add("Location", session.Url);
            _notyf.Success("Order has been paid successfully!");
            orderInfo.IsPayment = true;
            _dbContext.SaveChanges();
            return new StatusCodeResult(303);
        }
    }
}
