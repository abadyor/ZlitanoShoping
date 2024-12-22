using APPL.Contract.Cities;
using APPL.DTOs.Request.Cities;
using APPL.DTOs.Response.Cities;
using APPL.DTOS.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICity _cityRepo;

        public CitiesController(ICity cityRepo)
        {
            _cityRepo = cityRepo;
        }

        // إضافة مدينة جديدة
        [HttpPost("add-city")]
        public async Task<ActionResult<GeneralRespond>> Create(CreateCitiesRequestDTO model) =>
            Ok(await _cityRepo.CreateAsync(model));

        // الحصول على مدينة بواسطة ID
        [HttpGet("get-city/{id}")]
        public async Task<ActionResult<CitiesResponseDTO>> Get(int id) =>
            Ok(await _cityRepo.GetByIdAsync(id));

        // الحصول على جميع المدن
        [HttpGet("get-cities")]
        public async Task<ActionResult<IEnumerable<CitiesResponseDTO>>> Gets() =>
            Ok(await _cityRepo.GetAllAsync());

        // تحديث مدينة
        [HttpPut("update-city")]
        public async Task<ActionResult<GeneralRespond>> Update(UpdateCitiesRequestDTO model) =>
            Ok(await _cityRepo.UpdateAsync(model));

        // حذف مدينة
        [HttpDelete("delete-city/{id}")]
        public async Task<ActionResult<GeneralRespond>> Delete(int id) =>
            Ok(await _cityRepo.DeleteAsync(id));
    }
}
