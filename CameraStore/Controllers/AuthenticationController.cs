using CameraStore.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CameraStore.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthenticationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        
    }
}
