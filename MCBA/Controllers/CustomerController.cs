
﻿using Castle.Core.Resource;
using Microsoft.AspNetCore.Authorization;
using SimpleHashing.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCBA.Models;
using MCBA.Data;
using MCBA.ViewModels;
using X.PagedList;

namespace MCBA.Controllers;


public class CustomerController : Controller
{
    private readonly MCBAContext _context;


    // ReSharper disable once PossibleInvalidOperationException
    private int? CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();


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
                             .Include(x => x.Login)
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
            return RedirectToAction("MyProfile");
        }

        return NotFound();
    }
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Return the view with the existing validation error messages from data annotations
            return View(model);
        }

        var customer = await GetCustomer();

        if (!s_simpleHash.Verify(model.OldPassword, customer.Login.PasswordHash))
        {
            ModelState.AddModelError("CustomError", "The old password is incorrect.");
            return View(model);
        }
        string hashedNewPassword = s_simpleHash.Compute(model.NewPassword);

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("CustomError", "The entered passwords don't match.");
            return View(model);
        }

        customer.Login.PasswordHash = hashedNewPassword;
        await _context.SaveChangesAsync();

        return RedirectToAction("MyProfile");
    }


    public async Task<IActionResult> Deposit(int id)  // id -> AccountNumber
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == id);

        return View(
            new DepositAndWithdrawViewModel
            {
                AccountNumber = id,
                Account = account
            });
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(DepositAndWithdrawViewModel viewModel)
    {

        viewModel.Account = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == viewModel.AccountNumber);


        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
            return View(viewModel);
        }


        //Note this code could be moved out of the controller, e.g., into the Model.
        viewModel.Account.Balance += viewModel.Amount;
        viewModel.Account.Transactions.Add(
            new Transaction
            {
                TransactionType = 'D',
                Amount = viewModel.Amount,
                Comment = viewModel.Comment ?? "",
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Withdraw(int id)
    {
        // Routes to Withdraw Page

        var account = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == id);

        return View(
            new DepositAndWithdrawViewModel
            {
                AccountNumber = id,
                Account = account
            });
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw(DepositAndWithdrawViewModel viewModel)
    {
        // Validates withdrawal made
        // Redirects to Index (of CustomerController)

        viewModel.Account = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == viewModel.AccountNumber);


        // Query to check service charge limit
        int transactionCount = await _context.Transactions
            .CountAsync(t => t.AccountNumber == viewModel.AccountNumber &&
                            (t.TransactionType == 'W' || t.TransactionType == 'T') &&
                            t.DestinationAccountNumber != null);

        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
            return View(viewModel);
        }

        if ((viewModel.Account.Balance - viewModel.Amount) <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Not enough in balance.");
            return View(viewModel);

        }



        //Note this code could be moved out of the controller, e.g., into the Model.
        viewModel.Account.Balance -= viewModel.Amount;
        viewModel.Account.Transactions.Add(
            new Transaction
            {
                TransactionType = 'W',
                Amount = viewModel.Amount,
                Comment = viewModel.Comment ?? "",
                TransactionTimeUtc = DateTime.UtcNow
            });


        if (transactionCount >= 2)
        {
            viewModel.Account.Transactions.Add(
                new Transaction
                {
                    TransactionType = 'S',
                    Amount = 0.05M, // Withdrawal fee
                    Comment = viewModel.Comment ?? "",
                    TransactionTimeUtc = DateTime.UtcNow
                });
        }


        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Transfer(int id)
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == id);

        // GlobalAccounts: accounts except the current one
        var globalAccounts = await _context.Accounts
            .Where(a => a.AccountNumber != id)
            .ToListAsync();

        return View(new TransferViewModel
        {
            AccountNumber = id,
            Account = account,
            GlobalAccounts = globalAccounts
        });
    }



    [HttpPost] // Post request has to reinitialize viewModel
    public async Task<IActionResult> Transfer(TransferViewModel viewModel)
    {

        Console.WriteLine(viewModel.AccountNumber);
        Console.WriteLine(viewModel.DestinationAccountNumber);
        Console.WriteLine("----------------------------------------------------");

        // Load the source account with transactions
        viewModel.Account = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == viewModel.AccountNumber);

        viewModel.GlobalAccounts = await _context.Accounts
            .Where(a => a.AccountNumber != viewModel.AccountNumber)
            .ToListAsync();

        // Load the destination account with transactions
        var destinationAccount = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == viewModel.DestinationAccountNumber);


        // Validate transfer amount and destination account
        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
        }

        if (destinationAccount == null)
        {
            ModelState.AddModelError("DestinationAccountNumber", "Destination account not found.");
        }

        // Check if source account has sufficient funds
        if (viewModel.Account.Balance < viewModel.Amount)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient funds in the account.");
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }


        // Query to check service charge limit
        int transactionCount = await _context.Transactions
            .CountAsync(t => t.AccountNumber == viewModel.AccountNumber &&
                            (t.TransactionType == 'W' || t.TransactionType == 'T') &&
                            t.DestinationAccountNumber != null);

        decimal serviceFee = 0;
        if (transactionCount >= 2)
        {
            serviceFee = 0.05M; // service fee
            if (viewModel.Account.Balance < viewModel.Amount + serviceFee)
            {
                ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient funds in the account for transfer and service fee.");
                return View(viewModel);
            }

            // Apply service fee to the source account
            viewModel.Account.Balance -= serviceFee;
            viewModel.Account.Transactions.Add(
                new Transaction
                {
                    TransactionType = 'S',
                    Amount = serviceFee,
                    Comment = "Service fee for transfer",
                    TransactionTimeUtc = DateTime.UtcNow
                });
        }

        // Perform the transfer
        viewModel.Account.Balance -= viewModel.Amount;
        destinationAccount.Balance += viewModel.Amount;

        // Create transaction for source account
        viewModel.Account.Transactions.Add(
            new Transaction
            {
                TransactionType = 'T',
                Amount = viewModel.Amount,
                Comment = viewModel.Comment ?? "",
                TransactionTimeUtc = DateTime.UtcNow,
                DestinationAccountNumber = viewModel.DestinationAccountNumber
            });

        // Create transaction for destination account
        destinationAccount.Transactions.Add(
            new Transaction
            {
                TransactionType = 'T',
                Amount = viewModel.Amount,
                Comment = viewModel.Comment ?? "",
                TransactionTimeUtc = DateTime.UtcNow,
                DestinationAccountNumber = null
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Transfer));
    }

    public async Task<IActionResult> MyStatements()
    {
        var customer = await GetCustomer();
        return View(customer.Accounts);
    }
    public async Task<IActionResult> ViewMyStatementsList(int number, int page = 1)
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions) // Eager load the Transactions
            .FirstOrDefaultAsync(a => a.AccountNumber == number);

        if ( account == null) 
        {
            return Redirect("/");
        }
        const int pageSize = 4;
        var pagedList = await _context.Transactions.Where(x =>  x.AccountNumber == number)
            .OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);

        ViewBag.Account = account;
        return View(pagedList);
    }

}
