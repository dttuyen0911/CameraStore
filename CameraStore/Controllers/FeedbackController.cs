using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Create()
        {
            ViewData["customerID"] = new SelectList(_dbContext.Customers.ToList(), "customerID", "fullname");
            ViewData["proID"] = new SelectList(_dbContext.Products.ToList(), "proID", "proName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Feedback obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Feedbacks.Add(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["customerID"] = new SelectList(_dbContext.Customers.ToList(), "customerID", "fullname");
            ViewData["proID"] = new SelectList(_dbContext.Products.ToList(), "proID", "proName");
            return View(obj);
        }
        public IActionResult Edit(int id)
        {
            ViewData["customerID"] = new SelectList(_dbContext.Customers.ToList(), "customerID", "fullname");
            ViewData["proID"] = new SelectList(_dbContext.Products.ToList(), "proID", "proName");
            Feedback obj = _dbContext.Feedbacks.Find(id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(int id, Feedback obj)
        {
            if (ModelState.IsValid)
            {
                obj.feedID = id;
                _dbContext.Feedbacks.Update(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["customerID"] = new SelectList(_dbContext.Customers.ToList(), "customerID", "fullname");
            ViewData["proID"] = new SelectList(_dbContext.Products.ToList(), "proID", "proName");
            return View(obj);
        }
   
        public ActionResult Delete(int id)
        {
            var obj = _dbContext.Feedbacks.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _dbContext.Feedbacks.Remove(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult detailFeed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = _dbContext.Feedbacks.Find(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }
    }
}
