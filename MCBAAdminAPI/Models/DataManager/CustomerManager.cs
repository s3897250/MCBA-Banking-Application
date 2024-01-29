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

    // Retrieves all customers from the database.
    public async Task<IEnumerable<Customer>> GetAll()
    {
        return await _context.Customers.Include(c => c.Login).ToListAsync();

    }

    // Retrieves a specific customer by their ID.
    public async Task<Customer> GetCustomerById(int id)
    {
        var customer = await _context.Customers.Include(c => c.Login).FirstOrDefaultAsync(i => i.CustomerID == id);
        if (customer != null)
        {
            return customer;
        }

        return null;
    }

    // Updates the details of a specific customer.
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
            existingCustomer.State = customer.State;
            existingCustomer.PostCode = customer.PostCode;
            existingCustomer.TFN = customer.TFN;
            existingCustomer.Mobile = customer.Mobile;

            await _context.SaveChangesAsync();
        }
    }
}
