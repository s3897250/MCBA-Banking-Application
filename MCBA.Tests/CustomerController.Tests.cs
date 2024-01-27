using MCBA.Controllers;
using MCBA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MCBA.Tests;

public class CustomerControllerTests : IDisposable
{
    private readonly MCBAContext _context;

    public CustomerControllerTests()
    {
        // Using an in-memory SQLite database.
        // NOTE: The shared cache allows multiple connections to access the database.
        // NOTE: To support multiple tests running in parallel on separate in-memory databases the following
        // connection string can be used: $"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"
        _context = new MCBAContext(new DbContextOptionsBuilder<MCBAContext>().
            UseSqlite("Data Source=file::memory:?cache=shared").Options);

        // The EnsureCreated method creates the schema based on the current context model.
        _context.Database.EnsureCreated();

        SeedData.Initialize(_context);

        // You can read more about testing with databases here:
        // 
        // https://learn.microsoft.com/en-au/ef/core/testing/
        // https://learn.microsoft.com/en-au/ef/core/testing/choosing-a-testing-strategy#sqlite-as-a-database-fake

        // NOTE: The technique below is using the EF Core In-Memory provider - this provider was originally designed
        // to support internal testing of EF Core itself. Whilst it can be used it is now highly discouraged.
        // 
        // You can read more about this approach here:
        // https://learn.microsoft.com/en-au/ef/core/testing/choosing-a-testing-strategy#in-memory-as-a-database-fake
        // 
        // This approach uses the Microsoft.EntityFrameworkCore.InMemory package.

        // Seed data into the database using an in-memory instance of the context.
        //_context = new MCBAContext(new DbContextOptionsBuilder<MCBAContext>().
        //    UseInMemoryDatabase(nameof(MCBAContext)).Options);
        //SeedData.Initialize(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }



}
