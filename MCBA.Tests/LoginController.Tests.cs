using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MCBA.Controllers;
using MCBA.Data;

using Xunit;


namespace MCBA.Tests;
public class LoginControllerTests : IDisposable
{
    private readonly MCBAContext _context;
    private readonly LoginController _controller;
    private readonly HttpContext _httpContext;
    private readonly ISession _session;

    public LoginControllerTests()
    {
        var options = new DbContextOptionsBuilder<MCBAContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
        _context = new MCBAContext(options);
        SeedData.SeedTestData(_context);

        _session = new SimpleSession();
        _httpContext = new DefaultHttpContext { Session = _session };


        _controller = new LoginController(_context)
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
    public async Task Login_Successful_RedirectsToCustomerIndex()
    {
        // Arrange
        var loginID = "12345678"; // Valid login ID
        var password = "abc111";  // Valid password

        // Act
        var result = await _controller.Login(loginID, password);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Equal("Customer", redirectToActionResult.ControllerName);
    }

    [Fact]
    public async Task Login_Failed_ReturnsLoginViewWithModelError()
    {
        // Arrange
        var loginID = "invalidLoginID"; // InValid login ID
        var password = "invalidPassword"; // Invalid password

        // Act
        var result = await _controller.Login(loginID, password);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(_controller.ModelState.ContainsKey("LoginFailed"));
        Assert.Equal("Login failed, please try again.", _controller.ModelState["LoginFailed"].Errors[0].ErrorMessage);
    }

}
