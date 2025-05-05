using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagment.Models;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add(int eventId)
        {
            var task = new EventTask { EventId = eventId, Status = "Pending" };
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EventTask task)
        {
            Console.WriteLine("🔧 Add Task triggered");

            if (ModelState.IsValid)
            {
                Console.WriteLine($"✅ Valid Task: {task.Title}, EventId={task.EventId}");

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                Console.WriteLine("🟢 Task saved to DB");
                return RedirectToAction("Details", "Events", new { id = task.EventId });
            }

            Console.WriteLine("❌ Invalid Task ModelState:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($" - {error.ErrorMessage}");
            }

            return View(task);
        }
    }
}