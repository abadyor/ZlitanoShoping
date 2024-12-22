using APPL.Contract.Dictionarys;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public DictionaryController(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // استدعاء طريقة المستودع لجلب البيانات
                var result = await _dictionaryRepository.GetAllAsync();

                // إعادة النتيجة إلى العميل
                return Ok(result);
            }
            catch (Exception ex)
            {
                // معالجة أي خطأ
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddOrUpdateProduct")]
        public async Task<IActionResult> AddOrUpdateProduct(string productName, string Discription, string tableName)
        {
            try
            {
                await _dictionaryRepository.AddOrUpdateProductAsync(productName, Discription, tableName);
                return Ok("Product and table name processed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("searchProduct")]
        public async Task<IActionResult> SearchProduct(string searchTerm)
        {
            try
            {
                var results = await _dictionaryRepository.SearchProductsAsync(searchTerm);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

    }
}
