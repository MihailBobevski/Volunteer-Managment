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
            var role = HttpContext.Session.GetString("Role");
            if (role != "User")
                return RedirectToAction("Login", "User");

            var activeEvents = _context.Events
                .Where(e => e.Status == "Active")
                .Include(e => e.Tasks)
                .ThenInclude(t => t.User)
                .ToList();

            return View(activeEvents);
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
    }
}