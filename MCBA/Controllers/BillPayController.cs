using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCBA.Models;
using MCBA.Data;
using MCBA.ViewModels;

namespace MCBA.Controllers;


public class BillPayController : Controller
{
    private readonly MCBAContext _context;

    public BillPayController(MCBAContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index(int id)
    {
        var account = await _context.Accounts
                            .Include(a => a.BillPays)
                            .ThenInclude(bp => bp.Payee)
                            .FirstOrDefaultAsync(a => a.AccountNumber == id);

        if (account == null)
        {
            return NotFound(); // handle as appropriate
        }

        return View(account);
    }


    public IActionResult Create(int accountId)
    {
        var payeesList = _context.Payees.ToList();
        var viewModel = new CreateBillPayViewModel
        {
            AccountNumber = accountId,
            PayeeID = 0,
            ScheduleTimeUtc = DateTime.Now,
            Payees = payeesList // Assign the list of payees
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateBillPayViewModel viewModel)
    {

         // Potentially load account, then add BillPay that way


        // Validate that the amount is greater than 0
        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be greater than 0");

            viewModel.Payees = _context.Payees.ToList();
            return View(viewModel);
        }

        // Validate that the schedule time is in the future
        if (viewModel.ScheduleTimeUtc <= DateTime.UtcNow)
        {
            ModelState.AddModelError(nameof(viewModel.ScheduleTimeUtc), "Schedule time must be in the future");

            viewModel.Payees = _context.Payees.ToList();
            return View(viewModel);
        }


        var billPay = new BillPay
        {
            AccountNumber = viewModel.AccountNumber,
            PayeeID = viewModel.PayeeID,
            Amount = viewModel.Amount,
            ScheduleTimeUtc = viewModel.ScheduleTimeUtc,
            IsFailed = false,
            Processed = false,
            Period = viewModel.Period
        };

        _context.BillPays.Add(billPay);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", new { id = viewModel.AccountNumber });
    }

    public async Task<IActionResult> Cancel(int billPayId)
    {
        var billPay = await _context.BillPays.FindAsync(billPayId);

        if (billPay == null)
        {
            return NotFound();
        }

        // Check if the bill payment is already processed or not
        if (billPay.Processed)
        {
            return BadRequest("Cannot cancel a processed bill payment.");
        }

        _context.BillPays.Remove(billPay);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { id = billPay.AccountNumber });
    }


}
