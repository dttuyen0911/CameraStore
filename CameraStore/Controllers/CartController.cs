using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            return View(cart);
        }
        // Lấy tất cả các mục trong giỏ hàng của khách hàng
        public IActionResult GetCartItems(int customerId)
        {
            var cart = _dbContext.Carts.FirstOrDefault(c => c.customerID == customerId);
            if (cart == null)
            {
                return NotFound(); // Không tìm thấy giỏ hàng
            }

            var cartDetails = _dbContext.CartDetails.Where(cd => cd.cartID == cart.cartID).ToList();
            return Ok(cartDetails);
        }

        // Xóa toàn bộ giỏ hàng của khách hàng
        public IActionResult ClearCart(int customerId)
        {
            var cart = _dbContext.Carts.FirstOrDefault(c => c.customerID == customerId);
            if (cart == null)
            {
                return NotFound(); // Không tìm thấy giỏ hàng
            }

            _dbContext.CartDetails.RemoveRange(_dbContext.CartDetails.Where(cd => cd.cartID == cart.cartID));
            cart.cartQuantityTotal = 0;
            cart.cartPriceTotal = 0;
            _dbContext.SaveChanges();

            return Ok();
        }

        // Lưu giỏ hàng
        public IActionResult SaveCart(Cart cart)
        {
            // Lưu giỏ hàng
            _dbContext.Carts.Update(cart);
            _dbContext.SaveChanges();

            return Ok();
        }
        public IActionResult AddToCart(int customerId)
        {
            var cartItems = GetCartItems(customerId);
            if (cartItems is OkObjectResult result)
            {
                var cartDetails = result.Value as List<CartDetails>;
                return View("AddToCart", cartDetails); // Chuyển hướng đến action "AddToCart" và truyền dữ liệu giỏ hàng
            }
            else
            {
                return NotFound(); // Trả về NotFoundResult nếu không tìm thấy giỏ hàng
            }
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, decimal price)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Authentication");
            }

            // Lấy ID của người dùng từ cookie
            var customerIdClaim = User.FindFirst(ClaimTypes.Name);
            if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int customerId))
            {
                // Không thể xác định ID của khách hàng
                return BadRequest("Invalid customer ID");
            }

            // Tìm giỏ hàng của khách hàng
            var cart = _dbContext.Carts.FirstOrDefault(c => c.customerID == customerId);
            if (cart == null)
            {
                // Tạo giỏ hàng mới nếu chưa có
                cart = new Cart
                {
                    customerID = customerId,
                    cartQuantityTotal = 0,
                    cartPriceTotal = 0
                };

                _dbContext.Carts.Add(cart);
                _dbContext.SaveChanges(); // Lưu giỏ hàng mới vào cơ sở dữ liệu để có được cartID
            }

            // Tìm sản phẩm trong giỏ hàng
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
            _dbContext.SaveChanges();

            // Lấy lại thông tin giỏ hàng sau khi cập nhật
            var cartDetails = _dbContext.CartDetails.Where(cd => cd.cartID == cart.cartID).ToList();

            // Trả về view "Cart" với dữ liệu giỏ hàng
            return View("AddToCart", cartDetails);
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveCart(int cartDetailId)
        {
            var cartDetail = _dbContext.CartDetails.Find(cartDetailId);
            if (cartDetail == null)
            {
                return NotFound(); // Không tìm thấy mục giỏ hàng
            }

            var cart = _dbContext.Carts.Find(cartDetail.cartID);
            if (cart == null)
            {
                return NotFound(); // Không tìm thấy giỏ hàng
            }

            cart.cartQuantityTotal -= cartDetail.quantity;
            cart.cartPriceTotal -= cartDetail.price;
            _dbContext.CartDetails.Remove(cartDetail);
            _dbContext.SaveChanges();

            return Ok();
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public IActionResult UpdateCart(int cartDetailId, int quantity, decimal price)
        {
            var cartDetail = _dbContext.CartDetails.Find(cartDetailId);
            if (cartDetail == null)
            {
                return NotFound(); // Không tìm thấy mục giỏ hàng
            }

            var cart = _dbContext.Carts.Find(cartDetail.cartID);
            if (cart == null)
            {
                return NotFound(); // Không tìm thấy giỏ hàng
            }

            cart.cartQuantityTotal -= cartDetail.quantity;
            cart.cartPriceTotal -= cartDetail.price;

            cartDetail.quantity = quantity;
            cartDetail.price = price;

            cart.cartQuantityTotal += quantity;
            cart.cartPriceTotal += price;

            _dbContext.SaveChanges();

            return Ok();
        }
    }
}
