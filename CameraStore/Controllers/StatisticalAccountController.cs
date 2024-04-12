using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace CameraStore.Controllers
{
    [Authorize(Policy = "AdminPolicy")]

    public class StatisticalAccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public StatisticalAccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var accounts = _dbContext.Customers.ToList();

            return View(accounts);
        }
        public IActionResult GetAccountCountsByMonth()
        {
            int currentYear = DateTime.Now.Year;

            var accountCountsByMonth = new List<object>();

            for (int month = 1; month <= 12; month++)
            {
                var firstDayOfMonth = new DateTime(currentYear, month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var accountCount = _dbContext.Customers
                    .Where(a => a.createAt >= firstDayOfMonth && a.createAt <= lastDayOfMonth)
                    .Count();

                accountCountsByMonth.Add(new { Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month), AccountCount = accountCount });
            }

            return Json(accountCountsByMonth);
        }


    }
}
