using MCBAAdminAPI.Models.Repository;
using MCBAAdminAPI.Data;
using Microsoft.EntityFrameworkCore;
namespace MCBAAdminAPI.Models.DataManager;



public class CustomerManager : ICustomerRepository
{
    private readonly MCBAAdminContext _context;

    public CustomerManager(MCBAAdminContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAll()
    {
        return await _context.Customers.Include(c => c.Login).ToListAsync();

    }

    public async Task<Customer> GetCustomerById(int id)
    {
        var customer = await _context.Customers.Include(c => c.Login).FirstOrDefaultAsync(i => i.CustomerID == id);
        if (customer != null)
        {
            return customer;
        }

        return null;
    }

    public async Task UpdateCustomer(Customer customer)
    {
        var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerID == customer.CustomerID);

        if (existingCustomer != null)
        {
            // Update only specific fields of Customer
            existingCustomer.Name = customer.Name;
            existingCustomer.Address = customer.Address;
            existingCustomer.City = customer.City;
            existingCustomer.PostCode = customer.PostCode;

            // EF Core tracks changes and updates only the modified fields
            await _context.SaveChangesAsync();
        }
    }
}
