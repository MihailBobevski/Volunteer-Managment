using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteerManagment.Models;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdStr);

            var events = await _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.CreatedBy == userId)
                .ToListAsync();

            return View(events);
        }


        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Volunteers)
                .ThenInclude(ve => ve.User)
                .Include(e => e.Tasks) 
                .FirstOrDefaultAsync(m => m.EventId == id);


            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }
        
        [HttpPost]
        public IActionResult CompleteEvent(int eventId)
        {
            var ev = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (ev != null)
            {
                ev.Status = "Completed";
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = eventId });
        }
        [HttpPost]
        public async Task<IActionResult> CancelEvent(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Tasks)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev != null && ev.Status == "Active")
            {
                ev.Status = "Cancelled";

                foreach (var task in ev.Tasks)
                {
                    task.Status = "Cancelled";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = eventId });
        }

        [HttpPost]
        public async Task<IActionResult> ActivateEvent(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Tasks)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev != null && (ev.Status == "Completed" || ev.Status == "Cancelled"))
            {
                ev.Status = "Active";

                foreach (var task in ev.Tasks)
                {
                    if (task.Status == "Completed") continue;
                    task.Status = "Assigned";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = eventId });
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "User");

            @event.CreatedBy = int.Parse(userIdStr);
            @event.Status = "Active";

            if (!ModelState.IsValid)
                return View(@event);

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event updatedEvent)
        {
            if (id != updatedEvent.EventId)
                return NotFound();

            var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == id);
            if (existingEvent == null)
                return NotFound();

            updatedEvent.CreatedBy = existingEvent.CreatedBy;
            updatedEvent.Status = existingEvent.Status;

            if (!ModelState.IsValid)
                return View(updatedEvent);

            _context.Events.Update(updatedEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(@event => @event.Organizer)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Tasks)
                .Include(e => e.Volunteers)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event != null)
            {
                _context.Tasks.RemoveRange(@event.Tasks);
                _context.VolunteersEvents.RemoveRange(@event.Volunteers);
                _context.Events.Remove(@event);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
