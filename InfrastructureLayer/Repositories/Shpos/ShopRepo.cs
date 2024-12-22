using APPL.Contract.Shops;
using APPL.DTOs.Request.Shops;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOs.Response.Shops;
using APPL.DTOS.Response;
using Dapper;
using INFL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace INFL.Repositories.Shpos
{
    public class ShopRepo : IShops
    {
        private readonly ApplicationDbContext _context;
        private readonly PathSettings _path;
        public ShopRepo(ApplicationDbContext context, IOptions<PathSettings> path)
        {
            _context = context;
            _path = path.Value;
        }


        public async Task<int> CreateAsync(ShopsCreateDTO createDto)
        {
            try
            {
                string modifiedTableName = createDto.TableName.Replace("S_", "");
                string firstThreeCharacters = modifiedTableName.Substring(0, 3);

                // Retrieve the market based on the market code in ShopsCreateDTO
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
                if (market == null)
                {
                    throw new Exception("The market is null");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define the table name based on the market code
                string tableName = createDto.TableName;

                // Build the SQL command to insert the new record
                var insertSql = $@"
        INSERT INTO {tableName} (ProductName, Description, Price, DiscountedPrice, HasDiscount,TimeCreate)
        OUTPUT INSERTED.Id
        VALUES (@ProductName, @Description, @Price, @DiscountedPrice, @HasDiscount,@TimeCreate)";

                // Define the parameters for the SQL command
                var parameters = new[]
                {
            new SqlParameter("@ProductName", createDto.ProductName),
            new SqlParameter("@Description", createDto.Description),
            new SqlParameter("@Price", createDto.Price),
            new SqlParameter("@DiscountedPrice", createDto.DiscountedPrice),
            new SqlParameter("@HasDiscount", createDto.HasDiscount),
            new SqlParameter("@TimeCreate", DateTime.UtcNow)
        };

                // Open a database connection and execute the command
                var connection = dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = insertSql;
                    command.Parameters.AddRange(parameters);

                    var result = await command.ExecuteScalarAsync(); // Get the inserted ID
                    return Convert.ToInt32(result); // Convert the result to an integer ID
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }


        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null) return null;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return filePath;
        }



        public async Task<GeneralRespond> DeleteAsync(int id)
        {
            try
            {
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == "007");
                if (market == null)
                {
                    return new GeneralRespond(false, "Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define the table name based on the market code
                string tableName = market.Mcode; // Use the market code as the table name

                // Build the SQL command to delete the record
                var sql = $@"
                        DELETE FROM {tableName}
                        WHERE Id = @Id";

                // Execute the SQL command
                var parameters = new[]
                {
            new SqlParameter("@Id", id)
        };

                await dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                return new GeneralRespond(true);
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<IEnumerable<ShopsResponseDTO>> GetAllAsync(string marketCode)
        {
            try
            {
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
                if (market == null)
                {
                    throw new Exception("Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define the table name based on the market code
                string tableName = market.Mcode; // Use the market code as the table name

                // Build the SQL command to retrieve all records
                var sql = $@"
            SELECT Id, ProductName AS Name, '{market.Mcode}' AS Scode, '{market.MInstance}' AS SInstance, IsLock
            FROM {tableName}";

                // Execute the SQL command and map the result to ShopsResponseDTO
                var result = await dbContext.Set<ShopsResponseDTO>().FromSqlRaw(sql).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                // Handle exceptions, you could log them or rethrow
                throw new Exception($"An error occurred while fetching records: {ex.Message}");
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, int id)
        {
            // تحقق من صحة المعلمات
            if (file == null || file.Length == 0 || id == 0 || string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException("Invalid file, ID, or folder.");
            }

            // تعديل اسم المجلد في حال كانت S_ جزءًا من الاسم
            string modifiedTableName = folder.Replace("S_", "");
            string firstThreeCharacters = modifiedTableName.Substring(0, 3);

            // تحديد المسار بشكل مرن
            var uploadsPath = Path.Combine($"{_path.MarketPath}", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath); // إنشاء المجلد في حال عدم وجوده
            }

            // إنشاء اسم مميز للصورة
            var fileName = $"{id}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // رفع الصورة إلى المسار
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        /*  public async Task<string> UploadImageAsync(IFormFile file, string folder, int id)
          {
              if (file == null || file.Length == 0 || id == 0 || folder == null)
                  throw new ArgumentException("Invalid file or ID.");

              string modifiedTableName = folder.Replace("S_", "");
              string firstThreeCharacters = modifiedTableName.Substring(0, 3);

              var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");
              if (!Directory.Exists(uploadsPath))
              {
                  Directory.CreateDirectory(uploadsPath);
              }

              // اسم الصورة
              var fileName = $"{id}_{file.FileName}";
              var filePath = Path.Combine(uploadsPath, fileName);

              using (var stream = new FileStream(filePath, FileMode.Create))
              {
                  await file.CopyToAsync(stream);
              }

              return fileName;
          }*/

        //كود مهم جدا 111111111111111111111111111111111111111111111111111111111111111111111111111111111111
        //لحدف الصورة الاولي للعنصر ووضع الصورة الجديدة
        /*  public async Task<string> UploadImageAsync(IFormFile file, string folder, int id)
          {
              if (file == null || file.Length == 0 || id == 0 || folder == null)
                  throw new ArgumentException("Invalid file, ID, or folder.");

              string modifiedTableName = folder.Replace("S_", "");
              string firstThreeCharacters = modifiedTableName.Substring(0, 3);

              var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");
              if (!Directory.Exists(uploadsPath))
              {
                  Directory.CreateDirectory(uploadsPath);
              }

              // اسم الصورة الجديدة
              var newFileName = $"{id}_{file.FileName}";
              var newFilePath = Path.Combine(uploadsPath, newFileName);

              // حذف الصورة القديمة إذا كانت موجودة
              var oldFiles = Directory.GetFiles(uploadsPath, $"{id}_*"); // البحث عن أي صورة بنفس الـ ID
              foreach (var oldFile in oldFiles)
              {
                  try
                  {
                      File.Delete(oldFile);
                  }
                  catch (Exception ex)
                  {
                      // تسجيل الخطأ أو التعامل معه
                      throw new IOException($"Error deleting old file: {ex.Message}");
                  }
              }

              // حفظ الصورة الجديدة
              using (var stream = new FileStream(newFilePath, FileMode.Create))
              {
                  await file.CopyToAsync(stream);
              }

              return newFileName;
          }*/



        /* public async Task<GeneralRespond> GetImage(string tableName)
         {
             if (string.IsNullOrEmpty(tableName))
             {
                 return new GeneralRespond(false, "Table name is required.");
             }

             try
             {
                 // Retrieve the market based on the market code
                 var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == "007");
                 if (market == null)
                 {
                     return BadRequest("Market not found.");
                 }

                 // Build the connection string based on the retrieved instance name
                 string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                 var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                 // Initialize the DbContext with the dynamic connection string
                 using var dbContext = new MarketDbContext(dbContextOptions);

                 // Prepare the SQL query to select all records from the dynamic table
                 var query = $"SELECT Id, Item, Discription, Price, Quantity, NameImage FROM {tableName}";

                 // Establish a connection to the database
                 using (var connection = dbContext.Database.GetDbConnection())
                 {
                     await connection.OpenAsync(); // Open the connection

                     using (var command = connection.CreateCommand())
                     {
                         command.CommandText = query;

                         using (var result = await command.ExecuteReaderAsync())
                         {
                             var imageDataList = new List<GetImageResponse>();

                             while (await result.ReadAsync())
                             {
                                 // Read data from the result set
                                 var imageData = new GetImageResponse
                                 {
                                     Id = result["Id"] != DBNull.Value ? Convert.ToInt32(result["Id"]) : 0,
                                     Item = result["Item"].ToString(),
                                     Description = result["Discription"].ToString(),
                                     Price = result["Price"] != DBNull.Value ? Convert.ToDecimal(result["Price"]) : 0,
                                     Quantity = result["Quantity"] != DBNull.Value ? Convert.ToInt32(result["Quantity"]) : 0,
                                     ImageName = $"/uploads/{tableName}/{tableName}_{result["NameImage"]}"
                                 };
                                 imageDataList.Add(imageData);
                             }
                             return Ok(imageDataList); // Return the list of images
                         }
                     }
                 }
             }
             catch (SqlException sqlEx)
             {
                 // Log SQL errors or return a specific message
                 return StatusCode(500, $"Database error: {sqlEx.Message}");
             }
             catch (Exception ex)
             {
                 // Log general errors
                 return StatusCode(500, $"Internal server error: {ex.Message}");
             }
         }*/
        public Task<ShopsResponseDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateImageAsync(IFormFile file, string folder, int id, string nameold)
        {
            // تحقق من صحة المعلمات
            if (file == null || file.Length == 0 || id == 0 || string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException("Invalid file, ID, or folder.");
            }

            // تعديل اسم المجلد في حال كانت S_ جزءًا من الاسم
            string modifiedTableName = folder.Replace("S_", "");
            string firstThreeCharacters = modifiedTableName.Substring(0, 3);

            // تحديد المسار بشكل مرن
            var uploadsPath = Path.Combine($"{_path.MarketPath}", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath); // إنشاء المجلد في حال عدم وجوده
            }

            // تحديد اسم الصورة القديمة
            var oldImageName = nameold; // افترض أن الاسم القديم للصورة مرتبط بنفس الـ ID
            var oldImagePath = Path.Combine(uploadsPath, oldImageName);

            // إذا كانت الصورة القديمة موجودة، نقوم بحذفها
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            // إنشاء اسم مميز للصورة الجديدة
            var newFileName = $"{id}_{file.FileName}";
            var newFilePath = Path.Combine(uploadsPath, newFileName);

            // رفع الصورة الجديدة إلى المسار
            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return newFileName; // إرجاع اسم الصورة الجديدة
        }


        public void DeleteImage(string folder, string nameold)
        {
            // تحقق من صحة المعلمات
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(nameold))
            {
                throw new ArgumentException("Invalid folder or image name.");
            }

            // تعديل اسم المجلد في حال كانت S_ جزءًا من الاسم
            string modifiedTableName = folder.Replace("S_", "");
            string firstThreeCharacters = modifiedTableName.Substring(0, 3);

            // تحديد المسار بشكل مرن
            var uploadsPath = Path.Combine($"{_path.MarketPath}", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");
            if (!Directory.Exists(uploadsPath))
            {
                throw new DirectoryNotFoundException("The specified directory does not exist.");
            }

            // تحديد المسار الكامل للصورة
            var imagePath = Path.Combine(uploadsPath, nameold);

            // إذا كانت الصورة موجودة، قم بحذفها
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
                Console.WriteLine($"Image {nameold} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Image {nameold} not found.");
            }
        }

        public async Task<GeneralRespond> UpdateAsync(ShopsUpdateDTO updateDto)
        {
            try
            {
                string modifiedTableName = updateDto.TableName.Replace("S_", "");
                string firstThreeCharacters = modifiedTableName.Substring(0, 3);
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
                if (market == null)
                {
                    return new GeneralRespond(false, "Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define the table name based on the market code
                string tableName = market.Mcode; // Use the market code as the table name

                // Build the SQL command to update the record
                var sql = $@"

                UPDATE {tableName}
                SET ProductName = @ProductName,
                Description = @Description,
                Price = @Price,
                DiscountedPrice = @DiscountedPrice,
                HasDiscount = @HasDiscount
               
                WHERE Id = @Id";

                // Execute the SQL command
                var parameters = new[]
                {
            new SqlParameter("@ProductName", updateDto.ProductName),
            new SqlParameter("@Description", updateDto.Description),
            new SqlParameter("@Price", updateDto.Price),
            new SqlParameter("@DiscountedPrice", updateDto.DiscountedPrice),
            new SqlParameter("@HasDiscount", updateDto.HasDiscount),

            new SqlParameter("@Id", updateDto.Id)
        };

                await dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                return new GeneralRespond(true);
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<GeneralRespond> UpdateLockAsync(ShopsUpdateLockDTO shopsUpdateLockDTO)
        {
            try
            {
                string modifiedTableName = shopsUpdateLockDTO.TableName.Replace("S_", "");
                string firstThreeCharacters = modifiedTableName.Substring(0, 3);
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
                if (market == null)
                {
                    return new GeneralRespond(false, "Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define the table name based on the market code
                string tableName = market.Mcode; // Use the market code as the table name

                // Build the SQL command to update the IsLock field
                var sql = $@"
            UPDATE {tableName}
            SET IsLock = @IsLock
            WHERE Id = @Id";

                // Execute the SQL command
                var parameters = new[]
                {
            new SqlParameter("@IsLock", shopsUpdateLockDTO.IsLock),
            new SqlParameter("@Id", shopsUpdateLockDTO.Id)
        };

                await dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                return new GeneralRespond(true);
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"An error occurred: {ex.Message}");
            }
        }

        public Task<IEnumerable<ShopsResponseDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }


        /*      public async Task<Dictionary<int, ItemDataWithImagesDTO>> GetItemsDataWithImages(string tableName)
              {
                  var itemsDataWithImages = new Dictionary<int, ItemDataWithImagesDTO>();

                  // تعديل اسم الجدول لإزالة "S_"
                  string modifiedTableName = tableName.Replace("S_", "");

                  // أخذ أول ثلاثة حروف من اسم الجدول المعدل
                  string firstThreeCharacters = modifiedTableName.Substring(0, 3);

                  // جلب معلومات السوق بناءً على الكود
                  var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
                  if (market == null)
                  {
                      throw new Exception("Market not found.");
                  }

                  // إنشاء سلسلة الاتصال الديناميكية
                  string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                  var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                  // إنشاء DbContext باستخدام الاتصال الديناميكي
                  using var dbContext = new MarketDbContext(dbContextOptions);

                  // استعلام SQL لجلب جميع العناصر من الجدول الديناميكي
                  var query = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tableName}";

                  // جلب البيانات من الجدول الديناميكي
                  var itemsData = await dbContext.Set<ShopGetAllResponseDTO>()
                                                 .FromSqlRaw(query)
                                                 .ToListAsync();

                  // التكرار عبر جميع العناصر المسترجعة
                  foreach (var itemData in itemsData)
                  {
                      // بناء المسار الخاص بالصور بناءً على اسم الجدول المعدل
                      var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img_{modifiedTableName}");

                      // التحقق من وجود المجلد
                      List<string> images = new List<string>();
                      if (Directory.Exists(uploadsPath))
                      {
                          // جلب الصور التي تطابق النمط `{itemId}_*`
                          images = Directory.GetFiles(uploadsPath, $"{itemData.Id}_*")
                                            .Select(Path.GetFileName)
                                            .ToList();
                      }

                      // إضافة البيانات مع الصور في القاموس
                      itemsDataWithImages[itemData.Id] = new ItemDataWithImagesDTO
                      {
                          Id = itemData.Id,
                          ProductName = itemData.ProductName,
                          Description = itemData.Description,
                          Price = itemData.Price,
                          DiscountedPrice = itemData.DiscountedPrice,
                          HasDiscount = itemData.HasDiscount,
                          Images = images
                      };
                  }

                  return itemsDataWithImages;
              }*/

        public async Task<List<ItemDataWithImagesDTO>> GetItemsDataWithImages(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            // تعديل اسم الجدول لإزالة "S_" إذا كانت موجودة
            string modifiedTableName = tableName.Replace("S_", "");

            // أخذ أول ثلاثة حروف من اسم الجدول المعدل لتحديد السوق
            string firstThreeCharacters = modifiedTableName.Substring(0, 3);

            // جلب معلومات السوق بناءً على الكود
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
            if (market == null)
            {
                throw new Exception($"Market with code '{firstThreeCharacters}' not found.");
            }

            // إنشاء سلسلة الاتصال الديناميكية
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
            var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

            // إنشاء DbContext باستخدام الاتصال الديناميكي
            using var dbContext = new MarketDbContext(dbContextOptions);

            // تنفيذ استعلام SQL لجلب البيانات
            string query = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tableName}";
            var itemsData = new List<ItemDataWithImagesDTO>();

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                dbContext.Database.OpenConnection();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        itemsData.Add(new ItemDataWithImagesDTO
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            DiscountedPrice = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                            HasDiscount = reader.GetBoolean(5),
                            Images = new List<string>() // سيتم ملء الصور لاحقًا
                        });
                    }
                }
            }

            // إضافة الصور من المجلدات بناءً على اسم الجدول
            foreach (var item in itemsData)
            {
                // بناء المسار الخاص بالصور
                var uploadsPath = Path.Combine($"{_path.MarketPath}", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");

                // جلب أسماء الصور المطابقة للعنصر الحالي
                if (Directory.Exists(uploadsPath))
                {
                    var images = Directory.GetFiles(uploadsPath, $"{item.Id}_*")
                                          .Select(Path.GetFileName)
                                          .ToList();

                    item.Images = images;
                    item.path = uploadsPath;
                }

            }

            return itemsData;
        }

        public async Task<List<ItemDataWithImagesDTO>> GetItemsDataWithImagesWhereId(string tableName, int id)
        {
            if (string.IsNullOrWhiteSpace(tableName) && id == 0)
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            // تعديل اسم الجدول لإزالة "S_" إذا كانت موجودة
            string modifiedTableName = tableName.Replace("S_", "");

            // أخذ أول ثلاثة حروف من اسم الجدول المعدل لتحديد السوق
            string firstThreeCharacters = modifiedTableName.Substring(0, 3);

            // جلب معلومات السوق بناءً على الكود
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
            if (market == null)
            {
                throw new Exception($"Market with code '{firstThreeCharacters}' not found.");
            }

            // إنشاء سلسلة الاتصال الديناميكية
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
            var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

            // إنشاء DbContext باستخدام الاتصال الديناميكي
            using var dbContext = new MarketDbContext(dbContextOptions);

            // تنفيذ استعلام SQL لجلب البيانات
            string query = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tableName} Where Id={id}";
            var itemsData = new List<ItemDataWithImagesDTO>();

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                dbContext.Database.OpenConnection();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        itemsData.Add(new ItemDataWithImagesDTO
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            DiscountedPrice = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                            HasDiscount = reader.GetBoolean(5),
                            Images = new List<string>() // سيتم ملء الصور لاحقًا
                        });
                    }
                }
            }

            // إضافة الصور من المجلدات بناءً على اسم الجدول
            foreach (var item in itemsData)
            {
                // بناء المسار الخاص بالصور
                var uploadsPath = Path.Combine($"{_path.MarketPath}", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");

                // جلب أسماء الصور المطابقة للعنصر الحالي
                if (Directory.Exists(uploadsPath))
                {
                    var images = Directory.GetFiles(uploadsPath, $"{item.Id}_*")
                                          .Select(Path.GetFileName)
                                          .ToList();

                    item.Images = images;
                    item.path = uploadsPath;
                }

            }

            return itemsData;
        }








        public async Task UpdateItemData(string tableName, UpdateShopeDTO updatedItem)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            // تعديل اسم الجدول لإزالة "S_" إذا كانت موجودة
            string modifiedTableName = tableName.Replace("S_", "");

            // أخذ أول ثلاثة حروف من اسم الجدول المعدل لتحديد السوق
            string firstThreeCharacters = modifiedTableName.Substring(0, 3);

            // جلب معلومات السوق بناءً على الكود
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
            if (market == null)
            {
                throw new Exception($"Market with code '{firstThreeCharacters}' not found.");
            }

            // إنشاء سلسلة الاتصال الديناميكية
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

            var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

            // إنشاء DbContext باستخدام الاتصال الديناميكي
            using var dbContext = new MarketDbContext(dbContextOptions);

            // تنفيذ استعلام SQL لتحديث البيانات
            string query = $@"
                UPDATE {tableName}
                SET 
                    ProductName = @ProductName,
                    Description = @Description,
                    Price = @Price,
                    DiscountedPrice = @DiscountedPrice,
                    HasDiscount = @HasDiscount
                WHERE Id = @Id";

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;

                // إضافة المعلمات
                command.Parameters.Add(new SqlParameter("@ProductName", updatedItem.ProductName));
                command.Parameters.Add(new SqlParameter("@Description", updatedItem.Description));
                command.Parameters.Add(new SqlParameter("@Price", updatedItem.Price));
                command.Parameters.Add(new SqlParameter("@DiscountedPrice", (object)updatedItem.DiscountedPrice ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@HasDiscount", updatedItem.HasDiscount));
                command.Parameters.Add(new SqlParameter("@Id", updatedItem.Id));

                // فتح الاتصال وتنفيذ الاستعلام
                dbContext.Database.OpenConnection();
                await command.ExecuteNonQueryAsync();
            }

            // إذا كنت بحاجة إلى التعامل مع الصور، يمكنك إضافة الكود هنا لتعديل مسارات الصور أو تحديثها
        }



        /*  public async Task<List<ItemDataWithImagesDTO>> GetItemsDataWithImages(string tableName)
          {
              if (string.IsNullOrWhiteSpace(tableName))
                  throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

              // تعديل اسم الجدول لإزالة "S_" إذا كانت موجودة
              string modifiedTableName = tableName.Replace("S_", "");

              // أخذ أول ثلاثة حروف من اسم الجدول المعدل لتحديد السوق
              string firstThreeCharacters = modifiedTableName.Substring(0, 3);

              // جلب معلومات السوق بناءً على الكود
              var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
              if (market == null)
              {
                  throw new Exception($"Market with code '{firstThreeCharacters}' not found.");
              }

              // إنشاء سلسلة الاتصال الديناميكية
              string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
              var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

              // إنشاء DbContext باستخدام الاتصال الديناميكي
              using var dbContext = new MarketDbContext(dbContextOptions);

              // استعلام SQL لجلب البيانات من الجدول الديناميكي
              string query = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tableName}";

              // تنفيذ الاستعلام وجلب البيانات مباشرة
              var itemsData = await dbContext.Database.ExecuteSqlRawAsync(query, new object[] { })
                  .ContinueWith(t =>
                  {
                      // تحويل النتائج إلى DTO
                      return dbContext.Set<ItemDataWithImagesDTO>()
                                      .FromSqlRaw(query)
                                      .AsNoTracking()
                                      .ToList();
                  });

              // إضافة الصور من المجلدات بناءً على اسم الجدول
              foreach (var item in itemsData)
              {
                  // بناء المسار الخاص بالصور
                  var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img_{modifiedTableName}");

                  // جلب أسماء الصور المطابقة للعنصر الحالي
                  if (Directory.Exists(uploadsPath))
                  {
                      var images = Directory.GetFiles(uploadsPath, $"{item.Id}_*")
                                            .Select(Path.GetFileName)
                                            .ToList();

                      item.Images = images;
                  }
                  else
                  {
                      item.Images = new List<string>();
                  }
              }

              return itemsData;
          }*/


        /* public async Task<List<ItemDataWithImagesDTO>> GetItemsDataWithImages(string tableName)
         {
             if (string.IsNullOrWhiteSpace(tableName))
                 throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

             // تعديل اسم الجدول لإزالة "S_" إذا كانت موجودة
             string modifiedTableName = tableName.Replace("S_", "");

             // أخذ أول ثلاثة حروف من اسم الجدول المعدل لتحديد السوق
             string firstThreeCharacters = modifiedTableName.Substring(0, 3);

             // جلب معلومات السوق بناءً على الكود
             var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == firstThreeCharacters);
             if (market == null)
             {
                 throw new Exception($"Market with code '{firstThreeCharacters}' not found.");
             }

             // إنشاء سلسلة الاتصال الديناميكية
             string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
             var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

             // إنشاء DbContext باستخدام الاتصال الديناميكي
             using var dbContext = new MarketDbContext(dbContextOptions);

             // تنفيذ استعلام SQL لجلب البيانات من الجدول الديناميكي
             string query = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tableName}";

             var itemsData = await dbContext.Set<ItemDataWithImagesDTO>()
                                            .FromSqlRaw(query)
                                            .ToListAsync();

             // إضافة الصور من المجلدات بناءً على اسم الجدول
             foreach (var item in itemsData)
             {
                 // بناء المسار الخاص بالصور
                 var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img_{modifiedTableName}");

                 // جلب أسماء الصور المطابقة للعنصر الحالي
                 if (Directory.Exists(uploadsPath))
                 {
                     var images = Directory.GetFiles(uploadsPath, $"{item.Id}_*")
                                           .Select(Path.GetFileName)
                                           .ToList();

                     item.Images = images;
                 }
                 else
                 {
                     item.Images = new List<string>();
                 }
             }

             return itemsData;
         }*/

        /*    public async Task<Dictionary<int, ItemDataWithImagesDTO>> GetItemsDataWithImages(string tablename)
            {
                var itemsDataWithImages = new Dictionary<int, ItemDataWithImagesDTO>();

                // استعلام SQL لجلب جميع العناصر من الجدول الديناميكي
                var query = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tablename}";

                // استخدام FromSqlRaw بشكل صحيح مع الاستعلام
                var itemsData = await _context.Set<ShopGetAllResponseDTO>()
                                               .FromSqlRaw(query)
                                               .ToListAsync();

                // تعديل اسم الجدول لإزالة "S_"
                string modifiedTableName = tablename.Replace("S_", "");

                // أخذ أول ثلاثة حروف من اسم الجدول المعدل
                string firstThreeCharacters = modifiedTableName.Substring(0, 3);

                // التكرار عبر جميع العناصر المسترجعة
                foreach (var itemData in itemsData)
                {
                    // بناء المسار الخاص بالصور بناءً على اسم الجدول المعدل
                    var uploadsPath = Path.Combine("F:\\Databases", firstThreeCharacters, modifiedTableName, $"img{modifiedTableName}");

                    // التحقق من وجود المجلد
                    List<string> images = new List<string>();
                    if (Directory.Exists(uploadsPath))
                    {
                        // جلب الصور التي تطابق النمط `{itemId}_*`
                        images = Directory.GetFiles(uploadsPath, $"{itemData.Id}_*")
                                          .Select(Path.GetFileName)
                                          .ToList();
                    }

                    // إضافة البيانات مع الصور في القاموس
                    itemsDataWithImages[itemData.Id] = new ItemDataWithImagesDTO
                    {
                        Id = itemData.Id,
                        ProductName = itemData.ProductName,
                        Description = itemData.Description,
                        Price = itemData.Price,
                        DiscountedPrice = itemData.DiscountedPrice,
                        HasDiscount = itemData.HasDiscount,
                        Images = images
                    };
                }

                return itemsDataWithImages;
            }
    */



        // DTO للبيانات الأساسية للعنصر مع الصور
        /*    public class ItemDataWithImagesDTO
            {
                public int Id { get; set; }
                public string ProductName { get; set; }
                public string Description { get; set; }
                public decimal Price { get; set; }
                public decimal? DiscountedPrice { get; set; }
                public bool HasDiscount { get; set; }
                public List<string> Images { get; set; }  // قائمة الصور
            }*/

        public async Task<List<ShopeControllGetAllWithImageDTO>> GetAllShopeWhereSearch(string marketCode, string searchItem)
        {
            // Retrieve the market based on the market code
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
            if (market == null)
            {
                throw new Exception("Market not found.");
            }

            // Build the connection string based on the retrieved instance name
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

            // Table name for shop control
            var tableControl = $"C_{market.Mcode}";

            // SQL query to fetch shop data based on searchItem
            var query = $@"
                    SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], 
                           [Street], [NerestPoint], [NVistor], [Vendor_Id] , StartWork,  EndWork ,Notes
                    FROM {tableControl} 
                    WHERE [Item] = @searchItem";

            using var connection = new SqlConnection(connectionString);
            var shops = (await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(
                query, new { searchItem })).ToList();

            // Path for images
            var uploadsBasePath = Path.Combine($"{_path.MarketPath}", market.Mcode);

            // Add images to each shop
            foreach (var shop in shops)
            {
                // Construct the path to the folder for the shop's images
                var shopImagesPath = Path.Combine(uploadsBasePath, shop.Market_Code + shop.City + shop.Last_VendorCode, $"img{shop.Market_Code + shop.City + shop.Last_VendorCode}");

                // Check if the directory exists and retrieve the images
                if (Directory.Exists(shopImagesPath))
                {
                    var images = Directory.GetFiles(shopImagesPath, "*.*")
                                          .Select(Path.GetFileName)
                                          .ToList();

                    shop.Images = images; // Add images to the shop
                    shop.Path = shopImagesPath; // Save the path for reference
                }
                else
                {
                    shop.Images = new List<string>(); // No images found
                }
            }

            return shops;
        }

        /*    public async Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAll(string marketCode, string search)
            {
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
                if (market == null)
                {
                    throw new Exception("Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

                // Table name for shop control
                var tableControl = $"C_{market.Mcode}";

                // SQL query to fetch records from control table (C_{marketCode})
                var query = $"SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id] FROM {tableControl}";

                using var connection = new SqlConnection(connectionString);

                // Get all records from the control table
                var controlRecords = (await connection.QueryAsync(query)).ToList();

                // Skip the first record (if any)
                var recordsToProcess = controlRecords.Skip(1).ToList();

                // List to store the dynamic table names
                List<string> dynamicTableNames = new List<string>();

                // Generate dynamic table names based on the control records
                foreach (var record in recordsToProcess)
                {
                    string marketCodeFromRecord = record.Market_Code;
                    string city = record.City;
                    string lastVendorCode = record.Last_VendorCode;

                    // Build the dynamic table name
                    string dynamicTableName = $"S_{marketCodeFromRecord}{city}{lastVendorCode}";
                    dynamicTableNames.Add(dynamicTableName);
                }

                // List to store the result
                var shops = new List<ShopeControllGetAllWithImageDTO>();

                // Loop over each dynamic table name and search for the item in each one
                foreach (var tableName in dynamicTableNames)
                {
                    // Search query for the dynamic table
                    var searchQuery = $"SELECT [Id],[ProductName],[Description],[Price],[DiscountedPrice],[HasDiscount]  FROM {tableName} WHERE [ProductName] LIKE @search OR [Description] LIKE @search";

                    // Query the dynamic table based on the search parameter
                    var results = await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(searchQuery, new { search = "%" + search + "%" });

                    // Add results to the shops list
                    shops.AddRange(results);
                }

                // Path for images
                var uploadsBasePath = Path.Combine("F:\\Databases", market.Mcode);

                // Add images to each shop
                foreach (var shop in shops)
                {
                    // Construct the path to the folder for the shop's images
                    var shopImagesPath = Path.Combine(uploadsBasePath, $"{shop.Market_Code}{shop.City}{shop.Last_VendorCode}", $"img{shop.Market_Code}{shop.City}{shop.Last_VendorCode}");

                    // Check if the directory exists and retrieve the images
                    if (Directory.Exists(shopImagesPath))
                    {
                        var images = Directory.GetFiles(shopImagesPath, "*.*")
                                              .Select(Path.GetFileName)
                                              .ToList();

                        shop.Images = images; // Add images to the shop
                        shop.Path = shopImagesPath; // Save the path for reference
                    }
                    else
                    {
                        shop.Images = new List<string>(); // No images found
                    }
                }

                return shops;
            }*/


        public async Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAllFromMarket(string search)
        {
            var markets = await _context.Markets.ToListAsync();
            if (markets == null || !markets.Any())
            {
                throw new Exception("No markets found.");
            }

            var shops = new List<ShopeControllGetAllWithImageDTO>();

            string uploadsBasePath = $"{_path.MarketPath}";

            foreach (var market in markets)
            {
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var tableControl = $"C_{market.Mcode}";
                var query = $"SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock], StartWork,  EndWork ,Notes FROM {tableControl}";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var controlRecords = (await connection.QueryAsync(query)).ToList();
                var recordsToProcess = controlRecords.Skip(1).ToList();

                foreach (var record in recordsToProcess)
                {
                    string tableName = $"S_{record.Market_Code}{record.City}{record.Last_VendorCode}";
                    var searchQuery = $"SELECT [Id],[ProductName],[Description],[Price],[DiscountedPrice],[HasDiscount], @MarketCode AS Market_Code, @City AS City, @LastVendorCode AS Last_VendorCode FROM {tableName} WHERE [ProductName] LIKE @search OR [Description] LIKE @search";

                    var results = await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(
                        searchQuery,
                        new
                        {
                            search = "%" + search + "%",
                            MarketCode = record.Market_Code,
                            City = record.City,
                            LastVendorCode = record.Last_VendorCode
                        });

                    shops.AddRange(results);
                }
            }

            foreach (var shop in shops)
            {
                var shopImagesPath = Path.Combine(uploadsBasePath, shop.Market_Code, $"{shop.Market_Code}{shop.City}{shop.Last_VendorCode}", $"img{shop.Market_Code}{shop.City}{shop.Last_VendorCode}");
                if (Directory.Exists(shopImagesPath))
                {
                    var images = Directory.GetFiles(shopImagesPath, "*.*")
                                          .Select(Path.GetFileName)
                                          .ToList();

                    shop.Images = images;
                    shop.Path = shopImagesPath;
                }
                else
                {
                    shop.Images = new List<string>();
                }
            }

            return shops;
        }


        /*    public async Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAll(string marketCode, string search)
            {
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
                if (market == null)
                {
                    throw new Exception("Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

                // Table name for shop control
                var tableControl = $"C_{market.Mcode}";

                // SQL query to fetch records from control table
                var query = $"SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock] FROM {tableControl}";

                using var connection = new SqlConnection(connectionString);

                // Get all records from the control table
                var controlRecords = (await connection.QueryAsync(query)).ToList();

                // Skip the first record (if any)
                var recordsToProcess = controlRecords.Skip(1).ToList();

                // List to store the result
                var shops = new List<ShopeControllGetAllWithImageDTO>();

                // Path for images
                var uploadsBasePath = Path.Combine("F:\\Databases", market.Mcode);

                // Loop over each record in control table
                foreach (var record in recordsToProcess)
                {
                    // Generate dynamic table name
                    string tableName = $"S_{record.Market_Code}{record.City}{record.Last_VendorCode}";

                    // Search query for the dynamic table
                    var searchQuery = $"SELECT [Id],[ProductName],[Description],[Price],[DiscountedPrice],[HasDiscount], @MarketCode AS Market_Code, @City AS City, @LastVendorCode AS Last_VendorCode,@Name AS Name,@Region AS Region,@Street AS Street,@NerestPoint AS NerestPoint FROM {tableName} WHERE [ProductName] LIKE @search OR [Description] LIKE @search";

                    // Query the dynamic table based on the search parameter
                    var results = await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(
                        searchQuery,
                        new
                        {

                            search = "%" + search + "%",

                            MarketCode = record.Market_Code,
                            City = record.City,
                            LastVendorCode = record.Last_VendorCode,
                            Name = record.Name,
                            Region = record.Region,
                            Street = record.Street,
                            NerestPoint = record.NerestPoint
                        });

                    // Add results to the shops list
                    shops.AddRange(results);
                }

                // Add images to each shop
                foreach (var shop in shops)
                {
                    // Construct the path to the folder for the shop's images
                    var shopImagesPath = Path.Combine(uploadsBasePath, $"{shop.Market_Code}{shop.City}{shop.Last_VendorCode}", $"img{shop.Market_Code}{shop.City}{shop.Last_VendorCode}");

                    // Check if the directory exists and retrieve the images
                    if (Directory.Exists(shopImagesPath))
                    {
                        var images = Directory.GetFiles(shopImagesPath, "*.*")
                                              .Select(Path.GetFileName)
                                              .ToList();

                        shop.Images = images; // Add images to the shop
                        shop.Path = shopImagesPath; // Save the path for reference
                    }
                    else
                    {
                        shop.Images = new List<string>(); // No images found
                    }
                }

                return shops;
            }*/

        /*    public async Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAll(string marketCode, string search)
            {
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
                if (market == null)
                {
                    throw new Exception("Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

                // Table name for shop control
                var tableControl = $"C_{market.Mcode}";

                using var connection = new SqlConnection(connectionString);

                // Fetch records from the control table (skipping the first row)
                var controlQuery = $@"
        WITH CTE AS (
            SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id], [IsLock],
                   ROW_NUMBER() OVER (ORDER BY [Id]) AS RowNum
            FROM {tableControl}
        )
        SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id], [IsLock]
        FROM CTE
        WHERE RowNum > 1";

                var controlRecords = (await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(controlQuery)).ToList();

                // Path for images
                var uploadsBasePath = Path.Combine("F:\\Databases", market.Mcode);

                // Initialize result list
                var shops = new List<ShopeControllGetAllWithImageDTO>();

                foreach (var record in controlRecords)
                {
                    // Generate dynamic table name
                    string tableName = $"S_{record.Market_Code}{record.City}{record.Last_VendorCode}";

                    // Search query for the dynamic table
                    var searchQuery = $@"
            SELECT [Id], [ProductName] AS Name, [Description], [Price], [DiscountedPrice], [HasDiscount],
                   @MarketCode AS Market_Code, @City AS City, @LastVendorCode AS Last_VendorCode,
                   @Name AS Name, @Region AS Region, @Street AS Street, @NerestPoint AS NerestPoint

            FROM {tableName}
            WHERE [ProductName] LIKE @search OR [Description] LIKE @search";

                    // Execute the search query
                    var results = await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(searchQuery, new
                    {
                        search = "%" + search + "%",
                        MarketCode = record.Market_Code,
                        City = record.City,
                        LastVendorCode = record.Last_VendorCode,
                        Name = record.Name,
                        Region = record.Region,
                        Street = record.Street,
                        NerestPoint = record.NerestPoint,

                    });

                    // Add results to shops
                    shops.AddRange(results);
                }

                // Add images to each shop
                foreach (var shop in shops)
                {
                    // Construct the path to the folder for the shop's images
                    var shopImagesPath = Path.Combine(uploadsBasePath, $"{shop.Market_Code}{shop.City}{shop.Last_VendorCode}", $"img{shop.Market_Code}{shop.City}{shop.Last_VendorCode}");

                    // Check if the directory exists and retrieve the images
                    if (Directory.Exists(shopImagesPath))
                    {
                        var images = Directory.GetFiles(shopImagesPath, "*.*")
                                              .Select(Path.GetFileName)
                                              .ToList();

                        shop.Images = images; // Add images to the shop
                        shop.Path = shopImagesPath; // Save the path for reference
                    }
                    else
                    {
                        shop.Images = new List<string>(); // No images found
                    }
                }

                return shops;
            }*/

        public async Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAll(string marketCode, string search)
        {
            // Retrieve the market based on the market code
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
            if (market == null)
            {
                throw new Exception("Market not found.");
            }

            // Build the connection string
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

            // Table name for shop control
            var tableControl = $"C_{market.Mcode}";

            using var connection = new SqlConnection(connectionString);

            // Query to get shop control records
            var controlQuery = $@"
WITH CTE AS (
    SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id], [IsLock],StartWork,EndWork,Notes,
           ROW_NUMBER() OVER (ORDER BY [Id]) AS RowNum
    FROM {tableControl}
)
SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id], [IsLock],StartWork,EndWork,Notes
FROM CTE
WHERE RowNum > 1";

            var controlRecords = (await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(controlQuery)).ToList();

            // Path for images
            var uploadsBasePath = Path.Combine($"{_path.MarketPath}", market.Mcode);

            // Initialize result list
            var shops = new List<ShopeControllGetAllWithImageDTO>();

            foreach (var record in controlRecords)
            {
                // Generate dynamic table name for items
                string tableName = $"S_{record.Market_Code}{record.City}{record.Last_VendorCode}";

                // Search query to check if the table contains matching products
                var searchQuery = $@"
                SELECT TOP 1 [Id] -- Fetch only one record to check for existence
                FROM {tableName}
                WHERE [ProductName] LIKE @search OR [Description] LIKE @search";

                var productExists = await connection.QueryFirstOrDefaultAsync<int?>(searchQuery, new
                {
                    search = "%" + search + "%"
                });

                // If products matching the search term exist, add the shop
                if (productExists.HasValue)
                {
                    // Add shop details
                    var shop = new ShopeControllGetAllWithImageDTO
                    {
                        Id = record.Id,
                        Name = record.Name,
                        Market_Code = record.Market_Code,
                        Last_VendorCode = record.Last_VendorCode,
                        City = record.City,
                        Region = record.Region,
                        Street = record.Street,
                        NerestPoint = record.NerestPoint,
                        NVistor = record.NVistor,
                        Vendor_Id = record.Vendor_Id,
                        IsLock = record.IsLock,
                        StartWork = record.StartWork,
                        EndWork = record.EndWork,
                        Notes = record.Notes,
                    };

                    // Fetch images for the shop
                    var shopImagesPath = Path.Combine(uploadsBasePath, $"{shop.Market_Code}{shop.City}{shop.Last_VendorCode}", $"img{shop.Market_Code}{shop.City}{shop.Last_VendorCode}");
                    if (Directory.Exists(shopImagesPath))
                    {
                        shop.Images = Directory.GetFiles(shopImagesPath, "*.*")
                                               .Select(Path.GetFileName)
                                               .ToList();
                        shop.Path = shopImagesPath;
                    }
                    else
                    {
                        shop.Images = new List<string>(); // No images found
                    }

                    // Add shop to the results
                    shops.Add(shop);
                }
            }

            return shops;
        }



        // تأكد من استخدام هذه المكتبة

        public async Task<List<ShopeControllGetAllWithImageDTO>> GetAllShope1(string marketCode)
        {
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
            if (market == null)
            {
                throw new Exception("Market not found.");
            }

            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
            var tableControl = $"C_{market.Mcode}";

            var query = $@"
    WITH CTE AS (
        SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock], StartWork, EndWork, Notes,
               ROW_NUMBER() OVER (ORDER BY [Id]) AS RowNum
        FROM {tableControl}
    )
    SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock], StartWork, EndWork, Notes
    FROM CTE
    WHERE RowNum > 1";

            using var connection = new SqlConnection(connectionString);
            var shops = (await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(query)).ToList();

            foreach (var shop in shops)
            {
                // إذا كانت القيم موجودة كـ TimeSpan، يمكنك استخدام الحقول مباشرة.
                //var startWork = shop.StartWork;
                // var endWork = shop.EndWork;
                if (shop.StartWork.HasValue)
                {
                    Console.WriteLine($"StartWork: {shop.StartWork.Value.ToString("HH:mm")}");
                }
                else
                {
                    Console.WriteLine("StartWork is null");
                }

                if (shop.EndWork.HasValue)
                {
                    Console.WriteLine($"EndWork: {shop.EndWork.Value.ToString("HH:mm")}");
                }
                else
                {
                    Console.WriteLine("EndWork is null");
                }
                // إذا كنت بحاجة إلى تحويلها إلى TimeOnly (اختياري)
                // shop.StartTime = TimeOnly.FromTimeSpan(startWork);
                // shop.EndTime = TimeOnly.FromTimeSpan(endWork);

                // معالجة الصور
                var uploadsBasePath = Path.Combine($"{_path.MarketPath}", market.Mcode);
                var shopImagesPath = Path.Combine(uploadsBasePath, shop.Market_Code + shop.City + shop.Last_VendorCode, $"img{shop.Market_Code + shop.City + shop.Last_VendorCode}");

                if (Directory.Exists(shopImagesPath))
                {
                    var images = Directory.GetFiles(shopImagesPath, "*.*")
                                          .Select(Path.GetFileName)
                                          .ToList();

                    shop.Images = images;
                    shop.Path = shopImagesPath;
                }
                else
                {
                    shop.Images = new List<string>();
                }
            }


            return shops;
        }



        /*    public async Task<List<ShopeControllGetAllWithImageDTO>> GetAllShope1(string marketCode)
            {
                // Retrieve the market based on the market code
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
                if (market == null)
                {
                    throw new Exception("Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

                // Table name for shop control
                var tableControl = $"C_{market.Mcode}";

                // SQL query to fetch shop data
                // var query = $"SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id] FROM {tableControl}";
                var query = $@"
            WITH CTE AS (
                SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock], StartWork,  EndWork ,Notes,
                       ROW_NUMBER() OVER (ORDER BY [Id]) AS RowNum
                FROM {tableControl}
            )
            SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock], StartWork,  EndWork ,Notes
            FROM CTE
            WHERE RowNum > 1";

                using var connection = new SqlConnection(connectionString);
                var shops = (await connection.QueryAsync<ShopeControllGetAllWithImageDTO>(query)).ToList();

                // Path for images
                // Path for images
                var uploadsBasePath = Path.Combine($"{_path.MarketPath}", market.Mcode);

                // Add images to each shop
                foreach (var shop in shops)
                {
                    // تحويل القيم من string إلى TimeOnly
                    if (TimeOnly.TryParse(shop.StartWork.ToString(), out var startWork))
                    {
                        shop.StartWork = startWork;
                    }

                    if (TimeOnly.TryParse(shop.EndWork.ToString(), out var endWork))
                    {
                        shop.EndWork = endWork;
                    }

                    // معالجة الصور
                    var shopImagesPath = Path.Combine(uploadsBasePath, shop.Market_Code + shop.City + shop.Last_VendorCode, $"img{shop.Market_Code + shop.City + shop.Last_VendorCode}");

                    if (Directory.Exists(shopImagesPath))
                    {
                        var images = Directory.GetFiles(shopImagesPath, "*.*")
                                              .Select(Path.GetFileName)
                                              .ToList();

                        shop.Images = images;
                        shop.Path = shopImagesPath;
                    }
                    else
                    {
                        shop.Images = new List<string>();
                    }
                }

                return shops;

            }*/






        public async Task<List<ShopeControllResponseDTO>> GetAllShope(string marketCode)
        {
            // Retrieve the market based on the market code
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
            if (market == null)
            {
                throw new Exception("Market not found.");
            }

            // Build the connection string based on the retrieved instance name
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
            // var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

            // Initialize the DbContext with the dynamic connection string
            // using var dbContext = new MarketDbContext(dbContextOptions);

            var tableControl = $"C_{market.Mcode}";

            var query = $"SELECT   [Id],[Name] ,[Market_Code],[Last_VendorCode],[City],[Region],[Street],[NerestPoint],[NVistor],[Vendor_Id],StartWork,  EndWork ,[Notes] FROM {tableControl} ";
            using var connection = new SqlConnection(connectionString);
            // var result = await dbContext.Set<ShopeControllResponseDTO>().FromSqlRaw(query).ToListAsync();
            var result = await connection.QueryAsync<ShopeControllResponseDTO>(query);
            return result.ToList();
            //return result;

            /*    var shopDetails = new List<ShopGetAllResponseDTO>();
                foreach (var shop in result)
                {
                    // Generate dynamic table name based on the shop's data
                    var tableName = $"S_{shop.Market_Code}{shop.City}{shop.Last_VendorCode}";

                    // Query to fetch product details from the dynamically named table
                    var anotherQuery = $"SELECT [Id], [ProductName], [Description], [Price], [DiscountedPrice], [HasDiscount] FROM {tableName}";

                    var additionalData = await dbContext.Set<ShopGetAllResponseDTO>().FromSqlRaw(anotherQuery).ToListAsync();

                    // Map the shop data to the final response DTO
                    foreach (var product in additionalData)
                    {
                        shopDetails.Add(new ShopGetAllResponseDTO
                        {
                            Id = product.Id,                 // Product ID
                            ProductName = product.ProductName, // Product name
                            Description = product.Description, // Product description
                            Price = product.Price,            // Product price
                            DiscountedPrice = product.DiscountedPrice, // Discounted price (if any)
                            HasDiscount = product.HasDiscount // Discount flag
                        });
                    }

                }
            */


        }

        /*    public async Task<List<ShopeControllResponseDTO>> GetShopeByVendorAcrossAllMarkets(int vendorId)
            {
                // Retrieve all markets from the database
                var markets = await _context.Markets.ToListAsync();
                if (markets == null || !markets.Any())
                {
                    throw new Exception("No markets found.");
                }

                var result = new List<ShopeControllResponseDTO>();

                foreach (var market in markets)
                {
                    try
                    {
                        // Build the connection string dynamically for each market
                        string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                        var tableControl = $"C_{market.Mcode}";

                        var query = $"SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id],[IsLock] FROM {tableControl} WHERE Vendor_Id = @VendorId";

                        using var connection = new SqlConnection(connectionString);
                        var marketResult = await connection.QueryAsync<ShopeControllResponseDTO>(query, new { VendorId = vendorId });

                        result.AddRange(marketResult);
                    }
                    catch (Exception ex)
                    {
                        // Optionally log or handle errors for individual markets without interrupting the loop
                        Console.WriteLine($"Error processing market {market.Mcode}: {ex.Message}");
                    }
                }

                return result;
            }*/

        public async Task<List<ShopeControllWithMarketNameResponseDTO>> GetShopeByVendorAcrossAllMarkets(int vendorId)
        {
            // Retrieve all markets from the database
            var markets = await _context.Markets.ToListAsync();
            if (markets == null || !markets.Any())
            {
                throw new Exception("No markets found.");
            }

            var result = new List<ShopeControllWithMarketNameResponseDTO>();

            foreach (var market in markets)
            {
                try
                {
                    // Build the connection string dynamically for each market
                    string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                    var tableControl = $"C_{market.Mcode}";

                    var query = $"SELECT [Id], [Name], [Market_Code], [Last_VendorCode], [City], [Region], [Street], [NerestPoint], [NVistor], [Vendor_Id], [IsLock], StartWork,  EndWork ,[Notes] FROM {tableControl} WHERE Vendor_Id = @VendorId";

                    using var connection = new SqlConnection(connectionString);
                    var marketResult = await connection.QueryAsync<ShopeControllWithMarketNameResponseDTO>(query, new { VendorId = vendorId });

                    // Add MarketName to each item in marketResult
                    foreach (var shop in marketResult)
                    {
                        shop.MarketName = market.Name;
                    }

                    result.AddRange(marketResult);
                }
                catch (Exception ex)
                {
                    // Optionally log or handle errors for individual markets without interrupting the loop
                    Console.WriteLine($"Error processing market {market.Mcode}: {ex.Message}");
                }
            }

            return result;
        }


        /*   public async Task<List<ShopeControllResponseDTO>> GetShopeByVendor(string marketCode, int VendorId)
           {
               // Retrieve the market based on the market code
               var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
               if (market == null)
               {
                   throw new Exception("Market not found.");
               }

               // Build the connection string based on the retrieved instance name
               string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
               // var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

               // Initialize the DbContext with the dynamic connection string
               // using var dbContext = new MarketDbContext(dbContextOptions);

               var tableControl = $"C_{market.Mcode}";

               var query = $"SELECT   [Id],[Name] ,[Market_Code],[Last_VendorCode],[City],[Region],[Street],[NerestPoint],[NVistor],[Vendor_Id] FROM {tableControl} WHERE Vendor_Id={VendorId}";
               using var connection = new SqlConnection(connectionString);
               // var result = await dbContext.Set<ShopeControllResponseDTO>().FromSqlRaw(query).ToListAsync();
               var result = await connection.QueryAsync<ShopeControllResponseDTO>(query);
               return result.ToList();



           }*/


    }
}
