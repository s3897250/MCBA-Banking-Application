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
            return NotFound(); // Or handle as appropriate
        }

        return View(account);
    }


    public IActionResult Create(int accountId)
    {
        var viewModel = new CreateBillPayViewModel
        {
            AccountNumber = accountId,
            Payees = _context.Payees.ToList() // Assuming list of payees in context
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBillPayViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var billPay = new BillPay
            {
                AccountNumber = viewModel.AccountNumber,
                PayeeID = viewModel.PayeeID,
                Amount = viewModel.Amount,
                ScheduleTimeUtc = viewModel.ScheduleTimeUtc,
                Period = viewModel.Period
            };

            _context.Add(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { accountId = viewModel.AccountNumber });
        }
        viewModel.Payees = _context.Payees.ToList(); // Repopulate payees in case of form re-display
        return View(viewModel);
    }


}
