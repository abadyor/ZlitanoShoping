using APPL.Contract.Basket_ss;
using APPL.DTOs.Request.Basket_ss;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Basket_sController : ControllerBase
    {

        private readonly IBasket_sRepo _basket_sRepo;

        public Basket_sController(IBasket_sRepo basket_sRepo)
        {
            _basket_sRepo = basket_sRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBasket_s()
        {
            var result = await _basket_sRepo.GetAllBasket_sAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasket_sById(int id)
        {
            var result = await _basket_sRepo.GetBasket_sByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("GetByBasketId")]
        public async Task<IActionResult> GetBasket_sByBasketId(int basketId)
        {
            var result = await _basket_sRepo.GetBasket_sByBasketIdAsync(basketId);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddBasket_s([FromBody] AddBasket_sRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _basket_sRepo.AddBasket_sAsync(dto);
            return CreatedAtAction(nameof(GetBasket_sById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasket_s(int id, [FromBody] UpdateBasket_sRequestDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");

            var result = await _basket_sRepo.UpdateBasket_sAsync(dto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteBasket_s(int id)
        {
            var success = await _basket_sRepo.DeleteBasket_sAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
