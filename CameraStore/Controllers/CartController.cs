using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CameraStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Cart> cart = _dbContext.Carts.ToList();
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
            return View(cart);
        }

        // Hành động để hiển thị giỏ hàng của người dùng
       
        [HttpPost]
        // Sử dụng Authorize attribute để chỉ cho phép người dùng đã đăng nhập truy cập hành động này
        public IActionResult AddToCart(int productId, int quantity, decimal price)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value; // Lấy ID của người dùng từ cookie

            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var cart = _dbContext.Carts.FirstOrDefault(c => c.customerID == int.Parse(customerId));

            if (cart == null)
            {
                cart = new Cart
                {
                    customerID = int.Parse(customerId),
                    cartQuantityTotal = 0,
                    cartPriceTotal = 0
                };

                _dbContext.Carts.Add(cart);
                _dbContext.SaveChanges(); // Lưu giỏ hàng mới vào cơ sở dữ liệu để có được cartID
            }

            var existingCartItem = _dbContext.CartDetails.FirstOrDefault(cd => cd.cartID == cart.cartID && cd.proID == productId);

            if (existingCartItem != null)
            {
                existingCartItem.quantity += quantity;
                existingCartItem.price += price;
            }
            else
            {
                var newCartItem = new CartDetails
                {
                    cartID = cart.cartID, // Gán cartID đã có từ cart
                    proID = productId,
                    quantity = quantity,
                    price = price
                };

                _dbContext.CartDetails.Add(newCartItem);
            }

            cart.cartQuantityTotal += quantity;
            cart.cartPriceTotal += price;
            var product = _dbContext.Products.FirstOrDefault(p => p.proID == productId);
            if (product != null)
            {
                product.proQuantity -= quantity;
                _dbContext.SaveChanges();
            }
            else
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
            }
            _dbContext.SaveChanges();

            // Trả về thông tin sản phẩm đã thêm vào giỏ hàng dưới dạng JSON
            return Json(new { productId = productId, quantity = quantity, price = price });
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public IActionResult UpdateCart(int cartId, int quantity, decimal price)
        {
            var cartDetail = _dbContext.CartDetails.FirstOrDefault(cd => cd.cartID == cartId);
            if (cartDetail == null)
            {
                return NotFound(); // Không tìm thấy mục giỏ hàng
            }

            var cart = _dbContext.Carts.Find(cartDetail.cartID);
            if (cart == null)
            {
                return NotFound(); // Không tìm thấy giỏ hàng
            }

            // Kiểm tra số lượng mới với số lượng có sẵn của sản phẩm
            var product = _dbContext.Products.FirstOrDefault(p => p.proID == cartDetail.proID);
            if (product != null)
            {
                if (quantity > product.proQuantity)
                {
                    // Nếu số lượng mới lớn hơn số lượng có sẵn của sản phẩm,
                    // giảm số lượng mới xuống bằng số lượng có sẵn của sản phẩm
                    quantity = product.proQuantity;
                }
            }
            else
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
            }

            // Cập nhật số lượng và giá trong cartDetail
            cart.cartQuantityTotal -= cartDetail.quantity;
            cart.cartPriceTotal -= cartDetail.price;

            cartDetail.quantity = quantity;
            cartDetail.price = price;

            cart.cartQuantityTotal += quantity;
            cart.cartPriceTotal += price;

            _dbContext.SaveChanges();

            return Ok();
        }

        // Xóa mục trong giỏ hàng
        public IActionResult RemoveCart(int cartId)
        {
            var cartDetail = _dbContext.CartDetails.FirstOrDefault(cd => cd.cartID == cartId);
            if (cartDetail == null)
            {
                return NotFound(); // Không tìm thấy mục giỏ hàng
            }

            var cart = _dbContext.Carts.Find(cartDetail.cartID);
            if (cart == null)
            {
                return NotFound(); // Không tìm thấy giỏ hàng
            }

            // Cộng lại số lượng sản phẩm vào số lượng của sản phẩm trong cơ sở dữ liệu
            var product = _dbContext.Products.FirstOrDefault(p => p.proID == cartDetail.proID);
            if (product != null)
            {
                product.proQuantity += cartDetail.quantity;
                _dbContext.SaveChanges();
            }
            else
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
            }

            cart.cartQuantityTotal -= cartDetail.quantity;
            cart.cartPriceTotal -= cartDetail.price;
            _dbContext.CartDetails.Remove(cartDetail);
            _dbContext.SaveChanges();

            return Ok();
        }


    }
}

