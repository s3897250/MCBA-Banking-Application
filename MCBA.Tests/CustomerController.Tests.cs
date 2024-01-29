using MCBA.Controllers;
using MCBA.Data;
using MCBA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SimpleHashing.Net;
using System.ComponentModel.DataAnnotations;
namespace MCBA.Tests;

public class CustomerControllerTests : IDisposable
{
    private readonly MCBAContext _context;
    private readonly CustomerController _controller;
    private readonly HttpContext _httpContext;
    private readonly ISession _session;

    public CustomerControllerTests()
    {
        var options = new DbContextOptionsBuilder<MCBAContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
        _context = new MCBAContext(options);
        SeedData.SeedTestData(_context);

        _session = new SimpleSession();
        _httpContext = new DefaultHttpContext { Session = _session };

        // Set session ID - This is to assume that the customer with ID 2100 has already been logged in.
        _session.SetInt32(nameof(Customer.CustomerID), 2100); // Example Customer ID

        _controller = new CustomerController(_context)
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
    public async Task Index_ReturnsCustomerView_WithCustomerData()
    {
        // Arrange - No arrangement required as data is seeded

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Equal(2100, model.CustomerID);
    }


    [Fact]
    public async Task MyProfile_ReturnsCustomerView_WithCustomerData()
    {
        // Arrange

        // Act
        var result = await _controller.MyProfile();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Equal(2100, model.CustomerID);
    }


    [Theory]
    [InlineData("abc111", "newPass123", "newPass123")] // Valid change
    [InlineData("wrongOldPass", "newPass123", "newPass123")] // Incorrect old password
    [InlineData("abc111", "newPass123", "differentNewPass")] // New passwords don't match
    public async Task ChangePassword_Post_VariousScenarios(string oldPassword, string newPassword, string confirmPassword)
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            OldPassword = oldPassword,
            NewPassword = newPassword,
            ConfirmPassword = confirmPassword
        };
        ISimpleHash s_simpleHash = new SimpleHash();

        // Act
        var result = await _controller.ChangePassword(model);

        // Assert
        if (oldPassword == "abc111" && newPassword == confirmPassword)
        {
            // Valid scenario
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyProfile", redirectToActionResult.ActionName);

            var updatedCustomer = await _context.Customers.Include(c => c.Login).FirstOrDefaultAsync(c => c.CustomerID == 2100);
            Assert.True(s_simpleHash.Verify(newPassword, updatedCustomer.Login.PasswordHash));
        }
        else
        {
            // Invalid scenario
            Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }
    }

    [Theory]
    [InlineData("Shrey", "133 Droop Street", "Melbourne", "VIC", "3000", "123 456 789", "0412 345 678")]
    [InlineData("Singhal", "", "", "VIC", "3000", "123 456 789", "0412 345 678")]
    public async Task EditProfile_Post_Success(string name, string address, string city, string state, string postcode, string tfn, string mobile)
    {
        // Arrange
        var controller = new CustomerController(_context);
        var model = new EditProfileViewModel
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            PostCode = postcode,
            TFN = tfn,
            Mobile = mobile
        };
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
            }
        }

        // Act
        var result = await _controller.EditProfile(model);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyProfile", redirectToActionResult.ActionName);

        var updatedCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerID == 2100);
        Assert.Equal(name, updatedCustomer.Name);

    }



    [Theory]
    [InlineData("", "123 Street", "City", "VIC", "3000", "123 456 789", "0412 345 678")] // Invalid Name
    [InlineData("Shrey", "133 Droop Street", "Melbourn", "F", "3000", "123 456 789", "0412 345 678")] // Invalid State
    [InlineData("Shrey", "133 Droop Street", "Melbourn", "VIC", "3000", "123", "0412 345 678")] // Invalid TFN
    public async Task EditProfile_Post_Failure(string name, string address, string city, string state, string postcode, string tfn, string mobile)
    {
        // Arrange
        var controller = new CustomerController(_context);
        var model = new EditProfileViewModel
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            PostCode = postcode,
            TFN = tfn,
            Mobile = mobile
        };

        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
            }
        }

        // Act
        var result = await controller.EditProfile(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

}
