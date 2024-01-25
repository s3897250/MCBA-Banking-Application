namespace MCBAAdminAPI.Models.Repository
{
    public interface ICustomerRepository
    {
        Task <IEnumerable<Customer>> GetAll();
        Task<Customer> GetCustomerById(int id);
        Task UpdateCustomer(Customer customer);
        
    }
}
