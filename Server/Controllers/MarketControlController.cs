using APPL.Contract.MarketControl;
using APPL.DTOs.Request.ShopCntrole;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOS.Response;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MarketControlController : ControllerBase
{
    private readonly IMarketControl _marketControlRepo;

    public MarketControlController(IMarketControl marketControlRepo)
    {
        _marketControlRepo = marketControlRepo;
    }

    // POST: api/MarketControl/Create
    [HttpPost("Create")]
    public async Task<ActionResult<GeneralRespond>> Create([FromBody] ShopControleCreateDTO shopControle)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _marketControlRepo.CreateAsync(shopControle);
        if (result.Flag)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("Get/{shopId}")]
    public async Task<IActionResult> GetShopById(int shopId, string marketcode)
    {
        try
        {
            var shop = await _marketControlRepo.GetShopByIdAsync(shopId, marketcode);
            if (shop == null)
            {
                return NotFound(new { message = "Shop not found." });
            }
            return Ok(shop);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching the shop.", error = ex.Message });
        }
    }


    [HttpPut("UpdateIsLockShope")]
    public async Task<IActionResult> UpdateIsLockShope([FromQuery] int shopId, [FromQuery] string marketCode, [FromQuery] bool IsLock)
    {
        // التحقق من صحة المعاملات
        if (shopId <= 0)
        {
            return BadRequest("Invalid ShopId. It must be greater than zero.");
        }

        if (string.IsNullOrEmpty(marketCode))
        {
            return BadRequest("MarketCode cannot be null or empty.");
        }

        try
        {
            // استدعاء الدالة لتحديث عدد الزوار
            await _marketControlRepo.UpdateIsLockShope(shopId, marketCode, IsLock);

            // إرجاع استجابة ناجحة
            return Ok("Visitor count updated successfully.");
        }
        catch (Exception ex)
        {
            // إرجاع استجابة خطأ مع تفاصيل الخطأ
            return StatusCode(500, $"An error occurred while updating the visitor count: {ex.Message}");
        }
    }

    [HttpPut("UpdateVisitorCount")]
    public async Task<IActionResult> UpdateVisitorCount([FromQuery] int shopId, [FromQuery] string marketCode)
    {
        // التحقق من صحة المعاملات
        if (shopId <= 0)
        {
            return BadRequest("Invalid ShopId. It must be greater than zero.");
        }

        if (string.IsNullOrEmpty(marketCode))
        {
            return BadRequest("MarketCode cannot be null or empty.");
        }

        try
        {

            // استدعاء الدالة لتحديث عدد الزوار
            await _marketControlRepo.UpdateVisitorCount(shopId, marketCode);

            // إرجاع استجابة ناجحة
            return Ok("Visitor count updated successfully.");
        }
        catch (Exception ex)
        {
            // إرجاع استجابة خطأ مع تفاصيل الخطأ
            return StatusCode(500, $"An error occurred while updating the visitor count: {ex.Message}");
        }
    }

    // DELETE: api/MarketControl/Delete/{vendorId}
    [HttpDelete("Delete/{vendorId}")]
    public async Task<IActionResult> Delete(int vendorId)
    {
        var result = await _marketControlRepo.DeleteAsync(vendorId);
        if (result.Flag)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    // GET: api/MarketControl/GetAll
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<ShopControleResponseDTO>>> GetAll()
    {
        var shopControls = await _marketControlRepo.GetAllAsync();
        return Ok(shopControls);
    }

    // GET: api/MarketControl/GetLastCodeByCode/{shopCode}
    [HttpGet("GetLastCodeByCode/{shopCode}")]
    public async Task<ActionResult<ShopControle_LastCode_ResponseDTO>> GetLastCodeByCode(string shopCode)
    {
        var result = await _marketControlRepo.GetLastCode_ByCodeAsync(shopCode);
        if (result != null)
        {
            return Ok(result);
        }
        return NotFound("Market not found or no data available.");
    }

    // GET: api/MarketControl/GetVendorShops/{vendorId}
    [HttpGet("GetVendorShops/{vendorId}")]
    public async Task<ActionResult<List<ShopControleResponseBYID_DTO>>> GetVendorShops(int vendorId)
    {
        var vendorShops = await _marketControlRepo.GetVendorShopsAsync(vendorId);
        return Ok(vendorShops);
    }

    // PUT: api/MarketControl/Update
    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] ShopControleUpdateDTO shopControleUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _marketControlRepo.UpdateAsync(shopControleUpdate);
        if (result.Flag)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
