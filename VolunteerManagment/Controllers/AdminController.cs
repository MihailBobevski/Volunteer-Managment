using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult Requests()
    {
        var requests = _context.OrganizerRequests
            .Include(r => r.User)
            .OrderByDescending(r => r.DateRequested)
            .ToList();
        
        return View(requests);
    }

    [HttpPost]
    public IActionResult Accept(int id)
    {
        var req = _context.OrganizerRequests.FirstOrDefault(r => r.RequestId == id);
        if (req != null)
        {
            req.Status = "Accepted";
            var user = _context.Users.FirstOrDefault(u => u.UserId == req.UserId);
            if (user != null)
                user.RoleId = 2;
            _context.SaveChanges();
        }
        return RedirectToAction("Requests");
    }
    
    [HttpPost]
    public IActionResult Reject(int id)
    {
        var req = _context.OrganizerRequests.FirstOrDefault(r => r.RequestId == id);
        if (req != null)
        {
            req.Status = "Rejected";
            _context.SaveChanges();
        }
        
        return RedirectToAction("Requests");
    }
    
    [HttpGet]
    public IActionResult MotivationalLetter(int id)
    {
        var request = _context.OrganizerRequests
            .Include(r => r.User)
            .FirstOrDefault(r => r.RequestId == id);

        if (request == null)
            return NotFound();

        return View(request);
    }
    [HttpGet]
    public async Task<IActionResult> AllReports()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin") return Unauthorized();

        var reports = await _context.Reports
            .Include(r => r.User)
            .Include(r => r.Event)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(reports);
    }

    
}