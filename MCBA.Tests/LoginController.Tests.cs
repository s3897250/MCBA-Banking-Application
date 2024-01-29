using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MCBA.Controllers;
using MCBA.Data;
using MCBA.Models;
using System.Threading.Tasks;
using Xunit;
using SimpleHashing.Net;

namespace MCBA.Tests;
public class LoginControllerTests : IDisposable
{
    private readonly MCBAContext _context;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ISession> _mockSession;
    private readonly LoginController _controller;

    public LoginControllerTests()
    {
        var options = new DbContextOptionsBuilder<MCBAContext>()
            .UseInMemoryDatabase(databaseName: "TestLoginDb")
            .Options;
        _context = new MCBAContext(options);

        _mockHttpContext = new Mock<HttpContext>();
        _mockSession = new Mock<ISession>();
        _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

        _controller = new LoginController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            }
        };

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Check if data already exists
        if (!_context.Logins.Any())
        {
            // Add test data directly to the in-memory database
            var customer = new Customer
            {
                CustomerID = 2100,
                Name = "Matthew Bolger",
                
                Login = new Login
                {
                    LoginID = "1234567",
                    PasswordHash = new SimpleHash().Compute("abc111"),
                    CustomerID = 2100
                }
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
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
        var loginID = "1234567"; // Valid login ID
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
