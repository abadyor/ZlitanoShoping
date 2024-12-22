using APPL.Contract.Vendor;
using APPL.DTOs.Request.Vendor;
using APPL.DTOs.Response.Vendor;
using APPL.DTOS.Response;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController(IVendors _vendorRepository) : ControllerBase
    {

        [HttpPost("GetVendorWhereuserpasslogcode")]
        public async Task<ActionResult<VendorResponseDTO>> GetVendor([FromBody] VendorLoginRequest request)
        {
            try
            {
                var vendor = await _vendorRepository.GetVendorByUserNamePasswordLogUser(request.Username, request.Password, request.LogUser);
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

        // إضافة بائع جديد
        [HttpPost("add-vendor")]
        public async Task<ActionResult<GeneralRespond>> Create(VendorRequestCreateDTO model) =>
            Ok(await _vendorRepository.CreateAsync(model));

        // الحصول على بائع بواسطة ID
        [HttpGet("get-vendor/{id}")]
        public async Task<ActionResult<VendorResponse_Stores_DTO>> Get(int id) =>
            Ok(await _vendorRepository.GetByIdAsync(id));

        // الحصول على جميع البائعين
        [HttpGet("get-vendors")]
        public async Task<ActionResult<IEnumerable<VendorResponseDTO>>> Gets() =>
            Ok(await _vendorRepository.GetAllAsync());

        // تحديث بائع
        [HttpPut("update-vendor")]
        public async Task<ActionResult<GeneralRespond>> Update(VendorLogCode_UpdateDTO model) =>
            Ok(await _vendorRepository.UpdateLogCodeAsync(model)); // تأكد من وجود طريقة UpdateAsync في الواجهة IVendors

        // حذف بائع
        [HttpDelete("delete-vendor/{id}")]
        public async Task<ActionResult<GeneralRespond>> Delete(int id) =>
            Ok(await _vendorRepository.DeleteAsync(id));

        [HttpPost("login-vendor")]
        public async Task<ActionResult<GeneralRespond>> Login([FromBody] VendorLoginDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new GeneralRespond(false, "Username and password are required."));
            }

            var response = await _vendorRepository.VendorLoginAsync(model);

            if (!response.Flag)
            {
                return Unauthorized(response); // تستخدم Unauthorized للتعبير عن فشل تسجيل الدخول
            }

            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginVendor([FromBody] VendorLoginDTO request)
        {
            var vendor = await _vendorRepository.VendorUsernamePasswordAsync(request.Username, request.Password);

            if (vendor == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(vendor);
        }

    }
}
