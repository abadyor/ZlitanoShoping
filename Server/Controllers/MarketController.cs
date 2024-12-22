using APPL.Contract.Market;
using APPL.DTOs.Request.Market;
using APPL.DTOs.Response.Market;
using APPL.DTOS.Response;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly IMarekrt _marketRepository;

        public MarketController(IMarekrt marketRepository)
        {
            _marketRepository = marketRepository;
        }

        [HttpPost("AddMarket")]
        public async Task<IActionResult> AddMarket([FromBody] MarketTBCreatetDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _marketRepository.AddMarketAsync(request);

            if (result.Flag)
            {
                return Ok(new { message = result.Message });
            }
            else
            {
                return StatusCode(500, new { message = result.Message });
            }
        }



        // دالة لإنشاء جدول تحكم جديد داخل سوق معين
        [HttpPost("create-market-control/{marketCode}")]
        public async Task<ActionResult<GeneralRespond>> CreateMarketControl(string marketCode)
        {
            var result = await _marketRepository.CreateMarketControlAsync(marketCode);
            if (result.Flag == true)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }


        [HttpGet("GetAll")]
        public async Task<ActionResult<List<MarketGetAllResponseDTO>>> GetAll()
        {
            try
            {
                var markets = await _marketRepository.GetAllAsyncData();
                return Ok(markets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
