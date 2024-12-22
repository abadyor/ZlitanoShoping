using APPL.Contract.Customers;
using APPL.DTOs.Request.Customers;
using APPL.DTOs.Response.Customers;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomer _customerRepository;

        public CustomerController(ICustomer customerRepository)
        {
            _customerRepository = customerRepository;
        }



        [HttpPost("GetCustomerWhereuserpasslogcode")]
        public async Task<ActionResult<CustomerGetAllResponseDTO>> GetCustomer([FromBody] CustomerLoginResponseDTO request)
        {
            try
            {
                var vendor = await _customerRepository.GetVendorByUserNamePasswordLogUser(request.Username, request.Password, request.LogUser);
                if (vendor == null)
                {
                    return NotFound("Vendor not found");
                }

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                // Logging the error can be useful for debugging
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<CustomerGetAllResponseDTO>>> GetAll()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerGetByIdResponseDTO>> GetById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null) return NotFound($"Customer with ID {id} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Add(CustomerAddRequestDTO customerDto)
        {
            try
            {
                await _customerRepository.AddAsync(customerDto);
                return Ok("Customer added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(CustomerUpdateRequestDTO customerDto)
        {
            try
            {
                await _customerRepository.UpdateAsync(customerDto);
                return Ok("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _customerRepository.DeleteAsync(id);
                return Ok("Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Exists/{id}")]
        public async Task<ActionResult<bool>> CustomerExists(int id)
        {
            try
            {
                var exists = await _customerRepository.CustomerExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginCustomer([FromBody] CustomerLoginDTO request)
        {
            var vendor = await _customerRepository.CustomerUsernamePasswordAsync(request.Username, request.Password);

            if (vendor == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(vendor);
        }
    }
}
