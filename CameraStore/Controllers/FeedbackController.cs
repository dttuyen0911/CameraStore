using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(int id)
        {
            IEnumerable<Feedback> feedbacks = _dbContext.Feedbacks.ToList();
            return View(feedbacks);
        }
        public IActionResult Update(int id)
        {
            IEnumerable<Feedback> feedbacks = _dbContext.Feedbacks.ToList();
            return View(feedbacks);
        }
    }
}
