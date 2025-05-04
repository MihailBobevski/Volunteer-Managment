using Microsoft.AspNetCore.Mvc;
using VolunteerManagment2.Data;
using VolunteerManagment.Models;

namespace VolunteerManagment.Controllers;

public class OrganizerRequestController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrganizerRequestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Apply()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Apply([FromForm] string motivationalLetter)
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if(string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "User");
        
        int userId = int.Parse(userIdStr);

        var existing = _context.OrganizerRequests
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.DateRequested)
            .FirstOrDefault();
        if (existing != null && existing.Status == "Rejected" && (DateTime.Now - existing.DateRequested).TotalDays < 30)
        {
            ViewBag.Message = "You can reapply after 30 days.";
            return View();
        }

        var request = new OrganizerRequest
        {
            UserId = userId,
            MotivationalLetter = motivationalLetter,
            DateRequested = DateTime.Now,
            Status = "Pending"
        };
        _context.OrganizerRequests.Add(request);
        _context.SaveChanges();
        
        ViewBag.message = "Your application was submitted successfully!";
        return View();
    }
}