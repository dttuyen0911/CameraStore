using CameraStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CameraStore.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartViewComponent(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var customerId = user.FindFirst(ClaimTypes.Name)?.Value;

                if (customerId != null)
                {
                    var cart = await _dbContext.Carts
                        .Include(c => c.CartDetails)
                        .ThenInclude(cd => cd.Product)
                        .FirstOrDefaultAsync(c => c.customerID == int.Parse(customerId));

                    if (cart == null || cart.CartDetails.Count == 0)
                    {
                        ViewBag.Message = "No items in cart";
                        ViewBag.CartItemCount = 0;
                        return View(); // Trả về partial view mặc định nếu giỏ hàng trống
                    }

                    ViewBag.CartItemCount = cart.CartDetails.Count;
                    return View("CartPartialView", cart.CartDetails); // Trả về partial view chứa thông tin giỏ hàng
                }
            }

            ViewBag.CartItemCount = 0;
            return View(); // Trả về partial view mặc định nếu không có người dùng đăng nhập
        }

    }
}
