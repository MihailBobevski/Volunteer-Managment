using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagment2.Data;
using VolunteerManagment.Models;
using VolunteerManagment.Services;

namespace VolunteerManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITaskService _taskService;

        public HomeController(ApplicationDbContext context, ITaskService taskService)
        {
            _context = context;
            _taskService = taskService;
        }

        public IActionResult Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdStr);
            var role = HttpContext.Session.GetString("Role");

            List<Event> events;

            if (role == "User" || role == "Organizer")
            {
                events = _context.VolunteersEvents
                    .Where(ve => ve.UserId == userId && ve.Event.Status == "Active")
                    .Include(ve => ve.Event)
                    .ThenInclude(e => e.Organizer)
                    .Include(ve => ve.Event.Tasks)
                    .ThenInclude(t => t.User)
                    .Select(ve => ve.Event)
                    .ToList();
            }
            else
            {
                events = new List<Event>();
            }

            return View(events);
        }



        public IActionResult Details(int id)
        {
            var ev = _context.Events
                .Include(e => e.Tasks)
                .ThenInclude(t => t.User)
                .FirstOrDefault(e => e.EventId == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        [HttpPost]
        public IActionResult SignUpForTask(int taskId, int eventId)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "User");

            var success = _taskService.AssignTaskToUser(taskId, eventId, username);

            return RedirectToAction("Details", new { id = eventId });
        }

        public IActionResult PastEvents()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdStr);

            var events = _context.VolunteersEvents
                .Where(ve => ve.UserId == userId && ve.Event.Status == "Completed")
                .Include(ve => ve.Event)
                .ThenInclude(e => e.Organizer)
                .Select(ve => ve.Event)
                .Distinct()
                .ToList();

            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(events);
        }


        public IActionResult Volunteer()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdStr);

            var events = _context.Events
                .Include(e => e.Tasks)
                .Include(e => e.Organizer)
                .Where(e =>
                        e.Status == "Active" &&
                        e.CreatedBy != userId && 
                        !e.Tasks.Any(t => t.AssignedTo == userId) 
                )
                .ToList();

            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(events);
        }
    }
}