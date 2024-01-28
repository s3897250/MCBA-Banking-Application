using Microsoft.AspNetCore.Mvc;
using MCBA.Models;
using MCBA.Data;
using SimpleHashing;
using SimpleHashing.Net;
using Microsoft.EntityFrameworkCore;

namespace MCBA.Controllers;
public class LoginController : Controller
{
    private readonly MCBAContext _context;
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();

    public LoginController(MCBAContext context)
    {
        _context = context;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Logins
                          .Include(l => l.Customer)
                          .FirstOrDefaultAsync(l => l.LoginID == loginID);

        if (login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }
        else if (login.IsLocked)
        {
            ModelState.AddModelError("LoginFailed", "Your account is locked by the admin");
            return View(new Login { LoginID = loginID });
        }

        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return Redirect("/");
    }



}
