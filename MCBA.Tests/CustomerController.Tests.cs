using MCBA.Controllers;
using MCBA.Data;
using MCBA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;
using SimpleHashing;
using SimpleHashing.Net;
using System.Net.Http;
namespace MCBA.Tests;
public class CustomerControllerTests : IDisposable
{
    private readonly MCBAContext _context;
    private readonly CustomerController _controller;
    private readonly HttpContext _httpContext;

    public CustomerControllerTests()
    {
        var options = new DbContextOptionsBuilder<MCBAContext>()
            .UseInMemoryDatabase(databaseName: "TestCustomerDb")
            .Options;
        _context = new MCBAContext(options);

        // Use the new seeding method
        SeedData.SeedTestData(_context);

        _httpContext = new DefaultHttpContext();
        _controller = new CustomerController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            }
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    private void ConfigureSession(HttpContext httpContext, int customerId)
    {
        var session = new Mock<ISession>();
        var key = nameof(Customer.CustomerID);
        var value = BitConverter.GetBytes(customerId);

        // Mocking TryGetValue directly as it's used by GetInt32 internally
        session.Setup(_ => _.TryGetValue(key, out value)).Returns(true);

        // Setting up the session to be returned when HttpContext.Session is called
        httpContext.Session = session.Object;
    }

    [Fact]
    public async Task Index_ReturnsCustomerView_WithCustomerData()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        ConfigureSession(httpContext, 2100); // 2100 is the CustomerID
        _controller.ControllerContext.HttpContext = httpContext;

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Equal(2100, model.CustomerID);
    }
    // Test methods will go here
}








