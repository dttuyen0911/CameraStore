using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace CameraStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotyfService _notyf;
        public CartController(ApplicationDbContext dbContext, INotyfService notyf)
        {
            _dbContext = dbContext;
            _notyf = notyf;

        }
        [Authorize(Policy = "OwnerOrEmployeePolicy")]

        public IActionResult Index()
        {
            IEnumerable<Cart> cart = _dbContext.Carts.ToList();
            IEnumerable<CartDetails> cartDetails = _dbContext.CartDetails.Include(cd => cd.Product).ToList();
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
       
        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(int productId, int quantity, decimal price)
        {
            var customerId = User.FindFirst(ClaimTypes.Name)?.Value; 

            if (customerId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            if (quantity <= 0)
            {
                _notyf.Error("Quantity must be a positive number.");
                return Redirect($"/Home/productDetail/{productId}");
            }
            var product = _dbContext.Products.FirstOrDefault(p => p.proID == productId);

            if (quantity > product.proQuantity)
            {
                _notyf.Error("Insufficient product quantity in stock.");
            }
            else
            {
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
                    _dbContext.SaveChanges(); 
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
                        cartID = cart.cartID, 
                        proID = productId,
                        quantity = quantity,
                        price = price
                    };

                    _dbContext.CartDetails.Add(newCartItem);
                }

                cart.cartQuantityTotal += quantity;
                cart.cartPriceTotal += price;
                product.proQuantity -= quantity;
                product.proQuantitySold += quantity;

                if (product.proQuantity == 0)
                {
                    product.proStatus = "Sold out";
                }
                else if (product.proPercent != null && product.proPercent > 0)
                {
                    product.proStatus = "Sale";
                }
                else
                {
                    product.proStatus = "New";
                }
                _dbContext.SaveChanges();
                _notyf.Success("Add to cart successfully.");
            }
            return Redirect($"/Home/productDetail/{productId}");
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddToCart1(int productId, int quantity, decimal price)
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
                product.proQuantitySold += quantity;
                if (product.proQuantity == 0)
                {
                    product.proStatus = "Sold out";
                }
                else
                {
                    if (product.proPercent != null && product.proPercent > 0)
                    {
                        product.proStatus = "Sale";
                    }
                    else
                    {
                        product.proStatus = "New";
                    }
                }
                _dbContext.SaveChanges();
            }
            else
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
            }
            _dbContext.SaveChanges();
            _notyf.Success("Add to cart successfully.");
            return NoContent();
        }
        [HttpPost]
        [Authorize]
        public IActionResult UpdateCart(int cartId, int proId, int quantity)
        {
            if (quantity <= 0)
            {
                _notyf.Error("Quantity must be a positive number.");
                return NoContent();
            }

            var product = _dbContext.Products.FirstOrDefault(p => p.proID == proId);
            if (product == null)
            {
                return NotFound();
            }

            var cartDetail = _dbContext.CartDetails.FirstOrDefault(cd => cd.cartID == cartId && cd.proID == proId);
            if (cartDetail == null)
            {
                return NotFound();
            }

            var cart = _dbContext.Carts.Find(cartDetail.cartID);
            if (cart == null)
            {
                return NotFound();
            }

            int quantityDifference = quantity - cartDetail.quantity;

            // Check if the product stock can accommodate the new quantity
            if (quantityDifference > product.proQuantity)
            {
                _notyf.Error("Insufficient product quantity in stock.");
                return NoContent();
            }

            // Update product's sold quantity and stock
            product.proQuantitySold += quantityDifference;
            product.proQuantity -= quantityDifference;

            if (product.proQuantity == 0)
            {
                product.proStatus = "Sold out";
            }
            else
            {
                if (product.proPercent != null && product.proPercent > 0)
                {
                    product.proStatus = "Sale";
                }
                else
                {
                    product.proStatus = "New";
                }
            }

            // Update the cart details
            cart.cartQuantityTotal = cart.cartQuantityTotal - cartDetail.quantity + quantity;
            cartDetail.quantity = quantity;

            if (product.proSale != null && product.proSale > 0)
            {
                cartDetail.price = product.proSale;
            }
            else
            {
                cartDetail.price = product.proPrice;
            }

            // Calculate the new cart price total
            cart.cartPriceTotal = _dbContext.CartDetails
                .Where(cd => cd.cartID == cartId)
                .Sum(cd => cd.quantity * (cd.Product.proSale != null ? cd.Product.proSale : cd.Product.proPrice));

            _dbContext.SaveChanges();

            // Calculate the subtotal for the current cart
            var subtotal = _dbContext.CartDetails
                .Where(cd => cd.cartID == cartId)
                .Sum(cd => cd.quantity * (cd.Product.proSale != null ? cd.Product.proSale : cd.Product.proPrice));

            return Json(new { subtotal = subtotal });
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveCart(int cartId, int proId)
        {
            var cartDetail = _dbContext.CartDetails.FirstOrDefault(cd => cd.cartID == cartId && cd.proID == proId);
            if (cartDetail == null)
            {
                return NotFound();
            }

            var cart = _dbContext.Carts.Find(cartDetail.cartID);
            if (cart == null)
            {
                return NotFound();
            }

            // Lấy thông tin sản phẩm để cập nhật lại số lượng sản phẩm trong kho và tính lại tổng giá trị giỏ hàng
            var product = _dbContext.Products.FirstOrDefault(p => p.proID == cartDetail.proID);
            if (product != null)
            {
                product.proQuantitySold -= cartDetail.quantity;
                product.proQuantity += cartDetail.quantity;
                cart.cartPriceTotal -= cartDetail.price;
                if (product.proQuantity == 0)
                {
                    product.proStatus = "Sold out";
                }
                else
                {
                    if (product.proPercent != null && product.proPercent > 0)
                    {
                        product.proStatus = "Sale";
                    }
                    else
                    {
                        product.proStatus = "New";
                    }
                }
                _dbContext.SaveChanges();
            }
            else
            {
                return NotFound(); 
            }

            cart.cartQuantityTotal -= cartDetail.quantity; // Cập nhật lại tổng số lượng sản phẩm trong giỏ hàng
            _dbContext.CartDetails.Remove(cartDetail);
            _dbContext.SaveChanges();
            _notyf.Success("Remove product successfully.");

            // Kiểm tra xem giỏ hàng có trống sau khi xóa sản phẩm không
            var remainingCartDetails = _dbContext.CartDetails.Where(cd => cd.cartID == cartId).ToList();
            if (remainingCartDetails.Count == 0)
            {
                // Nếu không còn sản phẩm nào trong giỏ hàng, xóa giỏ hàng
                _dbContext.Carts.Remove(cart);
                _dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}

