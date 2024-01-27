using MCBAAdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace MCBAAdminPortal.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login() => View();
        [HttpPost]
        public IActionResult Login(Admin admin)
        {
            if (admin.username == "admin" && admin.password == "admin")
            {
                HttpContext.Session.SetString(nameof(Admin.username), admin.username);
                return RedirectToAction("Customers", "Admin");
            }
            ModelState.AddModelError("", "The username or the password is wrong.");
            return View();
        }
    }
}
