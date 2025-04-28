using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using VolunteerManagment.Models;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Controllers
{
    public class UserController : Controller
    {
        private  ApplicationDbContext _db;
       
        public UserController(ApplicationDbContext db)
        {
            _db = db;

        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email alredy registered.");
                }

                if (user.Password !=  Request.Form["ConfirmPassword"])
                {
                    ModelState.AddModelError("Password", "Passwords are not matching.");
                    return View(user);
                }
                user.Password = HashPassword(user.Password);
                _db.Users.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Login");   
                
            }
            return View(user);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Login(User user)
        {
            if (Request.Form["UserName"]!=""&& Request.Form["Password"] != "")
            {
                if(_db.Users.Any(u => u.UserName == user.UserName && u.Password == HashPassword(user.Password)))
                {
                    var User = _db.Users.FirstOrDefault(u => u.UserName == user.UserName && u.Password == HashPassword(user.Password));
                    if(User.RoleId == 1)
                    {
                        HttpContext.Session.SetString("Role","User");
                    }
                    if (User.RoleId == 2)
                    {
                        HttpContext.Session.SetString("Role", "Organizer");
                    }
                    if (User.RoleId == 3)
                    {
                        HttpContext.Session.SetString("Role", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login", "User"); 
        }
    }

}
