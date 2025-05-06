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
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool HasAccess()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin" || role == "Organizer";
        }

        private IActionResult BlockIfUnauthorized()
        {
            if (!HasAccess())
                return RedirectToAction("Login", "User");
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var block = BlockIfUnauthorized();
            if (block != null) return block;

            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdStr);

            List<Event> events;

            if (role == "Admin")
            {
                events = await _context.Events
                    .Include(e => e.Organizer)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            else
            {
                events = await _context.Events
                    .Include(e => e.Organizer)
                    .Where(e => e.CreatedBy == userId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }

            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var block = BlockIfUnauthorized();
            if (block != null) return block;

            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Volunteers)
                .ThenInclude(ve => ve.User)
                .Include(e => e.Tasks)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        [HttpPost]
        public IActionResult CompleteEvent(int eventId)
        {
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
            var block = BlockIfUnauthorized();
            if (block != null) return block;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
            var block = BlockIfUnauthorized();
            if (block != null) return block;

            if (id == null)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var block = BlockIfUnauthorized();
            if (block != null) return block;

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
