using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MCBA.Models;
using MCBA.Data;
using Microsoft.EntityFrameworkCore;

namespace MCBA.Controllers;

public class CustomerController : Controller
{
    private readonly MCBAContext _context;

    public CustomerController(MCBAContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {

        // Renders Customer object from Session/Context
        // Renders ATM Operations

        var customer = await _context.Customers.Include(x => x.Accounts).
            FirstOrDefaultAsync(x => x.CustomerID == 2100);

        return View(customer);
    }

    public async Task<IActionResult> Deposit(int id)
    {
        // Routes to Deposit Page
        return View(await _context.Accounts.FindAsync(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount)
    {
        // Validates deposit made
        // Redirects to Index (of CustomerController)

        var account = await _context.Accounts.Include(x => x.Transactions).
            FirstOrDefaultAsync(x => x.AccountNumber == id);
        
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");

        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

         //Note this code could be moved out of the controller, e.g., into the Model.
        account.Balance += amount;
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = 'D',
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }


    public async Task<IActionResult> Withdraw()
    {
        return View();
    }


    public async Task<IActionResult> Transfer()
    {
        return View();
    }


}
