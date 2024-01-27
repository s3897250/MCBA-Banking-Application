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
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDTO customerDto)
    {
        if (id != customerDto.CustomerID)
            return BadRequest("Mismatched customer ID");

        var customerToUpdate = new Customer
        {
            CustomerID = customerDto.CustomerID,
            Name = customerDto.Name,
            Address = customerDto.Address,
            City = customerDto.City,
            State = customerDto.State,
            PostCode = customerDto.PostCode,
            TFN = customerDto.TFN,
            Mobile = customerDto.Mobile

        };

        await _customerRepository.UpdateCustomer(customerToUpdate);
        return NoContent();
    }
}
