using APPL.Contract.Baskets;
using APPL.DTOs.Request.Basket_ss;
using APPL.DTOs.Request.Baskets;
using APPL.DTOs.Response.Baskets;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepo _basketRepo;

        public BasketController(IBasketRepo basketRepo)
        {
            _basketRepo = basketRepo;
        }

        // Get All Baskets
        [HttpGet]
        public async Task<IActionResult> GetAllBaskets()
        {
            var baskets = await _basketRepo.GetAllBasketsAsync();
            return Ok(baskets);
        }

        [HttpPost("GetBasketWhereuserpasslogcode")]
        public async Task<ActionResult<BasketGetAllResponseDTO>> GetBasket([FromBody] BasketCheckLogUserDTO request)
        {
            try
            {
                var vendor = await _basketRepo.GetBasketBybasketIdandCustomerIDLogUser(request.BasketId, request.CustomerId, request.LogUser);
                if (vendor == null)
                {
                    return NotFound("Vendor not found");
                }

                var updateclosebasket = await _basketRepo.updateCloseBasket(request.BasketId, true);
                if (updateclosebasket == "The Value Updated")
                {
                    return Ok(vendor);
                }
                else
                {
                    return BadRequest("No Update CloseBasket");
                }
            }
            catch (Exception ex)
            {
                // Logging the error can be useful for debugging
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get Basket By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasketById(int id)
        {
            var basket = await _basketRepo.GetBasketByIdAsync(id);
            if (basket == null)
                return NotFound();

            return Ok(basket);
        }

        // Add a New Basket
        [HttpPost("Create")]
        public async Task<IActionResult> AddBasket([FromBody] AddBasketRequsetDTO basketDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newBasket = await _basketRepo.AddBasketAsync(basketDto);
            return CreatedAtAction(nameof(GetBasketById), new { id = newBasket.Id }, newBasket);
        }

        // Update a Basket
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasket(int id, [FromBody] UpdateBasketRequestDTO basketDto)
        {
            if (id != basketDto.Id)
                return BadRequest("Basket ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedBasket = await _basketRepo.UpdateBasketAsync(basketDto);
            if (updatedBasket == null)
                return NotFound();

            return Ok(updatedBasket);
        }

        // Delete a Basket
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteBasket([FromQuery] int id)
        {
            var success = await _basketRepo.DeleteBasketAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // Get Baskets By CustomerId
        [HttpGet("CheckBasketForInsert")]
        public async Task<IActionResult> GetBasketsByCustomerId(int customerId)
        {
            var baskets = await _basketRepo.GetBasketsByCustomerIdAsync(customerId);
            return Ok(baskets);
        }

        [HttpPost("SendEmial")]
        public async Task<IActionResult> SendEmail(BasketSendEmailRequestDTO basketSendEmailRequestDTO)
        {
            var baskets = await _basketRepo.GetCustomerandSendEmailAndupdte(basketSendEmailRequestDTO.customerId, basketSendEmailRequestDTO.BasketId);
            return Ok(baskets);
        }
    }
}
