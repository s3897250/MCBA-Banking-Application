using Castle.Core.Resource;
using MCBA.Data;
using MCBA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MCBA.Controllers;
public class CustomerController : Controller
{
    private readonly MCBAContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(MCBAContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {

        // Eager loading.
        var customer = await _context.Customers.Include(x => x.Accounts).
            FirstOrDefaultAsync(x => x.CustomerID == CustomerID);

        return View(customer);
    }



}
