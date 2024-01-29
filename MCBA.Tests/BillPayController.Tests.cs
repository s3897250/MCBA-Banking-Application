using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MCBA.Controllers;
using MCBA.Data;
using MCBA.Models;
using MCBA.ViewModels;

namespace MCBA.Tests;

public class BillPayControllerTests : IDisposable
{
    private readonly MCBAContext _context;
    private readonly BillPayController _controller;
    private readonly HttpContext _httpContext;
    private readonly ISession _session;

    public BillPayControllerTests()
    {
        var options = new DbContextOptionsBuilder<MCBAContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
        _context = new MCBAContext(options);
        SeedData.SeedTestData(_context);

        _session = new SimpleSession();
        _httpContext = new DefaultHttpContext { Session = _session };

        _controller = new BillPayController(_context)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContext }
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Index_ReturnsViewWithAccountAndBillPays()
    {
        // Arrange
        var accountId = 4100;

        // Act
        var result = await _controller.Index(accountId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Account>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Equal(accountId, model.AccountNumber);
        Assert.NotNull(model.BillPays);
    }

    [Fact]
    public async Task Create_Post_SuccessfullyAddsBillPayAndRedirects()
    {
        // Arrange
        var viewModel = new CreateBillPayViewModel
        {
            AccountNumber = 4100,
            PayeeID = 1,
            Amount = 100,
            ScheduleTimeUtc = DateTime.UtcNow.AddDays(1),
            Period = 'M'
        };

        // Act
        var result = await _controller.Create(viewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Single(_context.BillPays.Where(bp => bp.AccountNumber == viewModel.AccountNumber && bp.Amount == viewModel.Amount));
    }

    [Fact]
    public async Task Cancel_Successful_RedirectsToIndex()
    {
        // Arrange
        var createViewModel = new CreateBillPayViewModel
        {
            AccountNumber = 4100, 
            PayeeID = 1,
            Amount = 50,
            ScheduleTimeUtc = DateTime.UtcNow.AddDays(5),
            Period = 'M'
        };

        await _controller.Create(createViewModel);

        var createdBillPay = _context.BillPays.OrderByDescending(bp => bp.BillPayID).FirstOrDefault();

        // Act
        var result = await _controller.Cancel(createdBillPay.BillPayID);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }


}