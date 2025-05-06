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

            if (ModelState.IsValid)
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Events", new { id = task.EventId });
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($" - {error.ErrorMessage}");
            }

            return View(task);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventTask task)
        {
            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("AllTasks", "Tasks",new { eventId = task.EventId });
            }
            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            int eventId = task.EventId; 

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("AllTasks","Tasks", new { eventId });        }

        
        [HttpGet]
        public async Task<IActionResult> AllTasks(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Tasks)
                .ThenInclude(t => t.User) 
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null)
            {
                return NotFound();
            }

            return View(ev);
        }
        
        [HttpGet]
        public async Task<IActionResult> AllEventTasks()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            var tasks = await _context.Tasks
                .Include(t => t.Event)
                .Include(t => t.User) 
                .OrderByDescending(t => t.Event.Date)
                .ToListAsync();

            return View(tasks);
        }

        
    }
}