using APPL.Contract.Shops;
using APPL.DTOs.Request.Shops;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOs.Response.Shops;
using APPL.DTOS.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly IShops _shops;

        public ShopsController(IShops shops)
        {
            _shops = shops;
        }

        // إضافة متجر جديد
        /*  [HttpPost("add-shop")]
          public async Task<ActionResult<GeneralRespond>> Create(ShopsCreateDTO model) =>
              Ok(await _shops.CreateAsync(model));*/

        [HttpPost("add-shop")]
        public async Task<ActionResult<int>> Create(ShopsCreateDTO model)
        {
            try
            {
                // استدعاء الدالة CreateAsync وإرجاع الـ Id المضاف
                var insertedId = await _shops.CreateAsync(model);

                // التحقق من أن الإدخال كان ناجحًا
                if (insertedId > 0)
                {
                    return Ok(insertedId); // إرجاع الـ Id عند النجاح
                }
                else
                {
                    return BadRequest("Failed to add the shop.");
                }
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /*      [HttpPost("UploadImage")]
              public async Task<ActionResult<GeneralRespond>> UploadImage([FromForm] IFormFile file, [FromQuery] string folder, int id)
              {
                  if (file == null || file.Length == 0 || id == 0)
                      return BadRequest("Invalid file.");



                  string modifiedTableName = folder.Replace("S_", "");
                  string firstThreeCharacters = modifiedTableName.Substring(0, 3);
                  *//*string picDirectoryPath = Path.Combine("F:\\Databases", market.Mcode, $"{market.Mcode}{ShopControle.City}{tableName}", $"img{market.Mcode}{ShopControle.City}{tableName}");*//*
                  var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");
                  if (!Directory.Exists(uploadsPath))
                  {
                      Directory.CreateDirectory(uploadsPath);
                  }

                  *//*            var uploadsPath = Path.Combine("wwwroot", "uploads", folder);*//*

                  // اسم الصورة
                  var fileName = $"{id}_{file.FileName}";
                  var filePath = Path.Combine(uploadsPath, $"{fileName}");

                  using (var stream = new FileStream(filePath, FileMode.Create))
                  {
                      await file.CopyToAsync(stream);
                  }

                  return Ok(new { fileName = file.FileName });
              }*/

        /*  [HttpPost("UploadImage")]
          public async Task<ActionResult<GeneralRespond>> UploadImage([FromForm] IFormFile file, [FromQuery] string folder, int id)
          {
              try
              {
                  var fileName = await _shops.UploadImageAsync(file, folder, id);
                  return Ok(new { fileName });
              }
              catch (ArgumentException ex)
              {
                  return BadRequest(ex.Message);
              }
              catch (Exception ex)
              {
                  return StatusCode(500, $"Internal server error: {ex.Message}");
              }
          }*/
        [HttpPost("UploadImages")]
        public async Task<ActionResult<GeneralRespond>> UploadImages([FromForm] List<IFormFile> files, [FromQuery] string folder, int id)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files were uploaded.");
            }

            if (string.IsNullOrWhiteSpace(folder) || id == 0)
            {
                return BadRequest("Invalid folder or ID.");
            }

            try
            {
                var fileNames = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        continue;
                    }

                    var fileName = await _shops.UploadImageAsync(file, folder, id);
                    fileNames.Add(fileName);
                }

                return Ok(new { fileNames });
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Argument Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        /*  [HttpPost("UploadImages")]
          public async Task<ActionResult<GeneralRespond>> UploadImages([FromForm] List<IFormFile> files, [FromQuery] string folder, int id)
          {
              // تحقق مبكر من صحة المعلمات
              if (files == null || files.Count == 0)
              {
                  return BadRequest("No files were uploaded.");
              }

              if (string.IsNullOrWhiteSpace(folder) || id == 0)
              {
                  return BadRequest("Invalid folder or ID.");
              }

              try
              {
                  var fileNames = new List<string>();

                  foreach (var file in files)
                  {
                      // تحقق من أن الملف ليس فارغًا
                      if (file.Length == 0)
                      {
                          continue; // تجاوز الملفات الفارغة
                      }

                      // رفع الصورة باستخدام دالة UploadImageAsync
                      var fileName = await _shops.UploadImageAsync(file, folder, id);
                      fileNames.Add(fileName);
                  }

                  return Ok(new { fileNames });
              }
              catch (ArgumentException ex)
              {
                  return BadRequest($"Argument Error: {ex.Message}");
              }
              catch (Exception ex)
              {
                  return StatusCode(500, $"Internal server error: {ex.Message}");
              }
          }*/

        [HttpGet("GetShopeByVendor")]
        public async Task<ActionResult<List<ShopeControllResponseDTO>>> GetShopeByVendor([FromQuery] int vendorId)
        {
            try
            {
                var result = await _shops.GetShopeByVendorAcrossAllMarkets(vendorId);
                if (result == null || !result.Any())
                {
                    return NotFound("No shops found for the specified vendor.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetAllShope")]
        public async Task<ActionResult<List<ShopeControllResponseDTO>>> GetAllShope(string marketCode)
        {
            try
            {
                // Call the GetAllShope function from the service
                var result = await _shops.GetAllShope(marketCode);

                // Check if no shops are found
                if (result == null || result.Count == 0)
                {
                    return NotFound(new { message = "No shops found for the specified market code." });
                }

                // Return the result with a 200 OK response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Return a 500 status code if something goes wrong
                return StatusCode(500, new { message = "An error occurred while retrieving shops.", details = ex.Message });
            }
        }

        /*    [HttpGet("GetAllShopeWithImage")]
            public async Task<ActionResult<List<ShopeControllGetAllWithImageDTO>>> GetAllShopeWithImage(string marketCode)
            {
                try
                {
                    // Call the GetAllShope1 function from the service
                    var result = await _shops.GetAllShope1(marketCode);

                    // Check if no shops are found
                    if (result == null || result.Count == 0)
                    {
                        return NotFound(new { message = "No shops found for the specified market code." });
                    }

                    // Return the result with a 200 OK response
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    // Return a 500 status code if something goes wrong
                    return StatusCode(500, new { message = "An error occurred while retrieving shops.", details = ex.Message });
                }
            }*/
        [HttpGet("GetAllShopeWithImage")]
        public async Task<ActionResult> GetAllShopeWithImage(string marketCode)
        {
            try
            {
                // Call the GetAllShope1 function from the service
                var result = await _shops.GetAllShope1(marketCode);

                if (result == null || result.Count == 0)
                {
                    return NotFound(new { message = "No shops found for the specified market code." });
                }

                return Ok(result); // Return only the data without wrapping it
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving shops.", details = ex.Message });
            }
        }



        [HttpPost("update-image")]
        public async Task<IActionResult> UpdateImageAsync([FromForm] IFormFile file, [FromForm] string folder, [FromForm] int id, [FromForm] string nameold)
        {
            try
            {
                if (file == null || file.Length == 0 || id == 0 || string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(nameold))
                {
                    return BadRequest("Invalid file, ID, or folder.");
                }

                // استدعاء الدالة UpdateImageAsync من الخدمة
                var newFileName = await _shops.UpdateImageAsync(file, folder, id, nameold);

                // إرجاع اسم الصورة الجديدة مع حالة النجاح
                return Ok(new { NewImageName = newFileName });
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء
                return StatusCode(500, new { Message = "An error occurred while updating the image.", Details = ex.Message });
            }
        }

        [HttpDelete("delete-image")]
        public IActionResult DeleteImage([FromQuery] string folder, [FromQuery] string nameold)
        {
            try
            {
                _shops.DeleteImage(folder, nameold);
                return Ok(new { Message = $"Image {nameold} deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (DirectoryNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("searchShopFromMarket")]
        public async Task<IActionResult> SearchShopsFromMarket([FromQuery] string search)
        {
            try
            {
                // Call the service method to get the shops
                var result = await _shops.SearchShopeAllFromMarket(search);

                if (result == null || !result.Any())
                {
                    return NotFound("No shops found matching the search criteria.");
                }

                // Return the list of shops as a response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Return an error response if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /*    [HttpGet("search")]
            public async Task<IActionResult> SearchShops([FromQuery] string marketCode, [FromQuery] string search)
            {
                if (string.IsNullOrEmpty(marketCode) || string.IsNullOrEmpty(search))
                {
                    return BadRequest("Both marketCode and search parameters are required.");
                }

                try
                {
                    // Call the service method to search shops
                    var shops = await _shops.SearchShopeAll(marketCode, search);

                    if (shops == null || !shops.Any())
                    {
                        return Ok(new { message = "No shops found matching the search criteria.", data = new List<ShopeControllGetAllWithImageDTO>() });
                    }

                    // Return the list of shops as the response
                    return Ok(shops);
                }
                catch (Exception ex)
                {
                    // Handle exception (log it if needed)
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
    */

        [HttpGet("search")]
        public async Task<IActionResult> SearchShops([FromQuery] string marketCode, [FromQuery] string search)
        {
            if (string.IsNullOrEmpty(marketCode) || string.IsNullOrEmpty(search))
            {
                return BadRequest("Both marketCode and search parameters are required.");
            }

            try
            {
                var shops = await _shops.SearchShopeAll(marketCode, search);

                if (shops == null || !shops.Any())
                {
                    return Ok("nodata");
                }

                return Ok(shops); // Return only the data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /* [HttpGet("search")]
         public async IActionResult SearchShops(string marketCode, string search)
         {
             var shops = _shops.SearchShopeAll(marketCode, search);

             if (shops == null || await !shops.Any())
             {
                 return Ok(new List<ShopeControllServiceDTO>());
             }

              return Ok(shops);
         }*/

        [HttpGet("GetImageForSRC")]
        public IActionResult GetImage([FromQuery] string imagePath)
        {
            try
            {
                // المسار الكامل للصورة
                string fullPath = Path.Combine(imagePath);

                // التحقق من وجود الملف
                if (System.IO.File.Exists(fullPath))
                {
                    // تحديد نوع MIME بناءً على امتداد الملف
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(fullPath, out string contentType))
                    {
                        contentType = "application/octet-stream"; // النوع الافتراضي إذا كان الامتداد غير معروف
                    }

                    // قراءة الملف وإرجاعه مع نوع MIME المناسب
                    var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                    return File(fileBytes, contentType); // إرجاع الصورة بالنوع المناسب
                }
                else
                {
                    return NotFound($"Image {imagePath} not found. in server yasenBono");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /*  [HttpGet("GetImageForSRC")]
          public IActionResult GetImage([FromQuery] string imagePath)
          {
              try
              {
                  string fullPath = Path.Combine(imagePath);
                  if (System.IO.File.Exists(fullPath))
                  {
                      var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                      return File(fileBytes, "image/PNG"); // أو النوع المناسب للصورة
                  }
                  else
                  {
                      return NotFound($"Image {imagePath} not found.");
                  }
              }
              catch (Exception ex)
              {
                  return StatusCode(500, $"An error occurred: {ex.Message}");
              }
          }*/

        [HttpGet("GetItemsDataWithImages")]
        public async Task<IActionResult> GetItemsDataWithImages([FromQuery] string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return BadRequest("Table name is required.");
            }

            try
            {
                var itemsDataWithImages = await _shops.GetItemsDataWithImages(tableName);

                if (itemsDataWithImages == null || !itemsDataWithImages.Any())
                {
                    return NotFound("No items found for the specified table.");
                }

                return Ok(itemsDataWithImages);
            }
            catch (Exception ex)
            {
                // إرجاع رسالة الخطأ
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetShopWhereId/{id}")]
        public async Task<IActionResult> GetItemDataWithImages(string tableName, [FromRoute] int id)
        {
            if (string.IsNullOrWhiteSpace(tableName) || id <= 0)
            {
                return BadRequest("Invalid table name or id.");
            }

            try
            {
                // استدعاء الخدمة للحصول على البيانات
                var itemsData = await _shops.GetItemsDataWithImagesWhereId(tableName, id);

                if (itemsData == null || itemsData.Count == 0)
                {
                    return NotFound(new { message = "Item not found." });
                }

                return Ok(itemsData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("UpdateShope")]
        public async Task<IActionResult> UpdateItem(string tableName, [FromBody] UpdateShopeDTO updatedItem)
        {
            if (updatedItem == null)
            {
                return BadRequest("Invalid item data.");
            }

            try
            {
                // استدعاء دالة التعديل في الخدمة
                await _shops.UpdateItemData(tableName, updatedItem);

                // العودة بالاستجابة
                return Ok(new { message = "Item updated successfully." });
            }
            catch (Exception ex)
            {
                // التعامل مع الاستثناءات
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // الحصول على متجر بواسطة ID
        [HttpGet("get-shop/{id}")]
        public async Task<ActionResult<ShopsResponseDTO>> Get(int id) =>
            Ok(await _shops.GetByIdAsync(id));

        // الحصول على جميع المتاجر
        [HttpGet("get-shops")]
        public async Task<ActionResult<IEnumerable<ShopsResponseDTO>>> Gets() =>
            Ok(await _shops.GetAllAsync());

        // تحديث متجر
        [HttpPut("update-shop")]
        public async Task<ActionResult<GeneralRespond>> Update(ShopsUpdateDTO model) =>
            Ok(await _shops.UpdateAsync(model));

        // حذف متجر
        [HttpDelete("delete-shop/{id}")]
        public async Task<ActionResult<GeneralRespond>> Delete(int id) =>
            Ok(await _shops.DeleteAsync(id));
    }
}
