using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CameraStore.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        public MessageController(ApplicationDbContext context)
        {
            _dbcontext = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create([FromForm] string chatName, [FromForm] string chatTelephone)
        {
            if (!string.IsNullOrEmpty(chatName) && !string.IsNullOrEmpty(chatTelephone))
            {
                try
                {
                    var chatbot = new Chatbot
                    {
                        chatName = chatName,
                        chatTelephone = chatTelephone
                    };
                    _dbcontext.Chatbots.Add(chatbot);
                    _dbcontext.SaveChanges();
                    return Ok("Information submitted successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
            else
            {
                return BadRequest("Full name and telephone are required.");
            }
        }
    }
}
