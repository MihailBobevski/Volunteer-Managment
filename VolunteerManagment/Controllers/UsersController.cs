using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagment.Models;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var volunteerEvents = await _context.VolunteersEvents
                .Include(ve => ve.Event)
                .Where(ve => ve.UserId == id)
                .ToListAsync();

            ViewBag.VolunteerEvents = volunteerEvents;

            return View(user);
        }

        public async Task<IActionResult> UserTasks(int? userId, int? eventId)
        {
            if (userId == null || eventId == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Where(t => t.AssignedTo == userId && t.EventId == eventId)
                .ToListAsync();

            if (tasks == null || tasks.Count == 0)
            {
                return NotFound();
            }

            ViewBag.IsOrganizer = HttpContext.Session.GetString("Role") == "Organizer";
            ViewBag.ViewedUserId = userId; 

            return View(tasks);
        }


        
        [HttpPost]
        public IActionResult UpdateTaskStatus(int taskId, int userId, int eventId, string status)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);

            if (task != null)
            {
                task.Status = status;
                _context.SaveChanges();

                if (status == "Completed")
                {
                    var volunteer = _context.VolunteersEvents
                        .FirstOrDefault(ve => ve.UserId == userId && ve.EventId == eventId);

                    if (volunteer != null)
                    {
                        volunteer.HoursContributed += 1;

                        var totalTasks = _context.Tasks
                            .Count(t => t.EventId == eventId && t.AssignedTo == userId);
                        var completedTasks = _context.Tasks
                            .Count(t => t.EventId == eventId && t.AssignedTo == userId && t.Status == "Completed");

                        if (totalTasks > 0 && completedTasks == totalTasks)
                        {
                            volunteer.Status = "Completed";
                        }

                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("UserTasks", new { userId, eventId });
        }





        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FName,LName,Email,Password,Phone,CreatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FName,LName,Email,Password,Phone,CreatedAt")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
