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
    private int? CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

    public CustomerController(MCBAContext context)
    {
        _context = context;
    }

    private async Task<Customer> GetCustomer()
    {
        var customerID = CustomerID;

        if (!customerID.HasValue)
        {
            return null;
        }
        return await _context.Customers
                             .Include(x => x.Accounts)
                             .FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
    }

    public async Task<IActionResult> Index()
    {
        var customer = await GetCustomer();
        return View(customer);
    }


    public async Task<IActionResult> MyProfile()
    {
        var customer = await GetCustomer();

        if (customer == null)
        {
            return Redirect("/");

        }
        return View(customer);
    }

    // GET: Display the Edit Profile form
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var customer = await GetCustomer();

        if (customer == null)
        {
            return NotFound();
        }

        var model = new EditProfileViewModel
        {
            Name = customer.Name,
            Address = customer.Address,
            City = customer.City,
            PostCode = customer.PostCode
        };

        return View(model);
    }

    // POST: Process the Edit Profile form submission
    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var customer = await GetCustomer();

        if (customer != null)
        {
            customer.Name = model.Name;
            customer.Address = model.Address;
            customer.City = model.City;
            customer.PostCode = model.PostCode;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return NotFound();
    }





}
