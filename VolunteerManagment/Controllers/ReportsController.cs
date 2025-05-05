using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagment.Models;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Write(int eventId, string actionType)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "User");
            }

            var ev = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (ev == null)
            {
                return NotFound();
            }

            ViewBag.EventTitle = ev.Title;
            ViewBag.ActionType = actionType;

            var report = new Report
            {
                EventId = eventId,
                UserId = int.Parse(userIdStr),
                Content = ""
            };

            return View(report);
        }
        
        [HttpPost]
        public async Task<IActionResult> Write(Report report, string actionType)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdStr); 

            var oldReports = _context.Reports.Where(r => r.EventId == report.EventId);
            _context.Reports.RemoveRange(oldReports);

            report.UserId = userId;

            var ev = await _context.Events.FindAsync(report.EventId);
            if (ev == null)
                return NotFound("Event not found.");
            report.EventId = ev.EventId;

            _context.Reports.Add(report);

            ev.Status = actionType == "Complete" ? "Completed" : "Cancelled";

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Events", new { id = report.EventId });
        }
        

        [HttpPost]
        public async Task<IActionResult> Activate(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Tasks)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null) return NotFound();

            var oldReports = _context.Reports.Where(r => r.EventId == eventId);
            _context.Reports.RemoveRange(oldReports);

            ev.Status = "Active";

            foreach (var task in ev.Tasks)
            {
                if (task.Status != "Completed")
                {
                    task.Status = "Assigned";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        [HttpGet]
        public async Task<IActionResult> View(int eventId)
        {
            var report = await _context.Reports
                .Include(r => r.User)
                .Include(r => r.Event)
                .ThenInclude(e => e.Tasks)
                .ThenInclude(t => t.User) 
                .FirstOrDefaultAsync(r => r.EventId == eventId);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }


        
    }
}
