using MCBAAdminAPI.Models.DataManager;
using MCBAAdminAPI.Models.Repository;
using MCBAAdminAPI.Data;
using MCBAAdminAPI.Models;
using Microsoft.AspNetCore.Mvc;
namespace MCBAAdminAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<Customer>> GetAll()
    {
        return await _customerRepository.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<Customer> GetCustomerByName(int id)
    {
        return await _customerRepository.GetCustomerById(id);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCustomer(int id, Customer customer)
    {
        if (id != customer.CustomerID)
            return BadRequest();

        _customerRepository.UpdateCustomer(customer);
        return NoContent();
    }
}
