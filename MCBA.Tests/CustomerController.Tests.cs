using MCBA.Controllers;
using MCBA.Models;
using MCBA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace MCBA.Tests;

public class CustomerControllerTests : IDisposable
{
    private readonly MCBAContext _context;

    public CustomerControllerTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<MCBAContext>(options =>
            options.UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"));

        var serviceProvider = services.BuildServiceProvider();
        SeedData.Initialize(serviceProvider);

        _context = serviceProvider.GetRequiredService<MCBAContext>();
        _context.Database.EnsureCreated();
    }

        public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }



}
