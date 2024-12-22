using APPL.DTOs.Request.Customers;
using APPL.DTOs.Response.Customers;

namespace APPL.Contract.Customers
{
    public interface ICustomer
    {
        Task<List<CustomerGetAllResponseDTO>> GetAllAsync();
        Task<CustomerGetByIdResponseDTO> GetByIdAsync(int id);
        Task AddAsync(CustomerAddRequestDTO customer);
        Task UpdateAsync(CustomerUpdateRequestDTO customer);
        public Task<CustomerGetAllResponseDTO> GetVendorByUserNamePasswordLogUser(string username, string password, string loguser);
        Task DeleteAsync(int id);
        Task<bool> CustomerExistsAsync(int id);
        public Task<string> GenerateCodeAndSendEmail(string recipientEmail);
        public Task<string> updateonefild(string code);
        public Task<CustomerGetAllResponseDTO?> CustomerUsernamePasswordAsync(string username, string password);
    }
}
