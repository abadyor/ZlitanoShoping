using APPL.Contract.MarketControl;
using APPL.DTOs.Request.ShopCntrole;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOS.Response;
using Dapper;
using INFL.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace INFL.Repositories.MarketControl
{
    public class MarketControlRepo : IMarketControl
    {
        private readonly ApplicationDbContext _context;
        private readonly PathSettings _path;
        public MarketControlRepo(ApplicationDbContext context, IOptions<PathSettings> path)
        {
            _context = context;
            _path = path.Value;
        }
        public async Task<string> GetLastVendorCodeAsync(string mcode, string instance, ShopControleCreateDTO shopData)
        {
            string connectionString2 = $"Server={instance};Database={mcode};Trusted_Connection=True;TrustServerCertificate=True;";
            var dbContextOptions2 = MarketDbContextFactory.CreateDbContextOptions(connectionString2);

            using var dbContext2 = new MarketDbContext(dbContextOptions2);

            string tableName = $"C_{mcode}".Trim();
            string _FileQuery = $"SELECT Last_VendorCode FROM [{tableName}] WHERE Id = 1";

            var lastCode = await dbContext2.LastCodeQuery
                .FromSqlRaw(_FileQuery)
                .AsNoTracking()  // لتجنب تتبع الكيان
                .FirstOrDefaultAsync();


            string lastVendorCode = lastCode?.Last_VendorCode ?? "001";

            // توليد الكود الجديد
            string newCode = CodeGenerator.GenerateNextCode(lastVendorCode);

            // تحديث الكود في قاعدة البيانات باستخدام معامل مباشرةً
            await dbContext2.Database.ExecuteSqlRawAsync(
                $"UPDATE [{tableName}] SET Last_VendorCode = '{newCode}' WHERE Id = 1");


            string insertQuery = $"INSERT INTO [{tableName}] (Name, Market_Code, Last_VendorCode, City, Region, Street, NerestPoint, NVistor, Vendor_Id,StartWork,EndWork,Notes) " +
                        "VALUES (@Name, @Market_Code, @Last_VendorCode, @City, @Region, @Street, @NerestPoint, @NVistor, @Vendor_Id,@StartWork,@EndWork,@Notes)";

            var parameters = new[]
            {
                new SqlParameter("@Name", shopData.Name),
                new SqlParameter("@Market_Code", shopData.Market_Code),
                new SqlParameter("@Last_VendorCode", newCode), // استخدام الكود الجديد
                new SqlParameter("@City", shopData.City),
                new SqlParameter("@Region", shopData.Region),
                new SqlParameter("@Street", shopData.Street),
                new SqlParameter("@NerestPoint", shopData.NerestPoint),
                new SqlParameter("@NVistor", shopData.NVistor),
                new SqlParameter("@Vendor_Id", shopData.Vendor_Id),
                 new SqlParameter("@StartWork", (object?)shopData.StartWork?.ToTimeSpan() ?? DBNull.Value),
                 new SqlParameter("@EndWork", (object?)shopData.EndWork?.ToTimeSpan() ?? DBNull.Value),
                 new SqlParameter("@Notes", shopData.Notes)
            };

            // تنفيذ استعلام الإدخال لإضافة السجل الجديد
            await dbContext2.Database.ExecuteSqlRawAsync(insertQuery, parameters);


            return newCode;
        }


        public async Task<GeneralRespond> CreateAsync(ShopControleCreateDTO ShopControle)
        {
            try
            {

                // Retrieve the market based on the market code in ShopsCreateDTO
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == ShopControle.Market_Code);
                if (market == null)
                {
                    return new GeneralRespond(false, "Market not found.");
                }

                // Build the connection string based on the retrieved instance name
                string connectionString = $"Server={market.MInstance};Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define the table name based on the market code
                string tableName = await GetLastVendorCodeAsync(ShopControle.Market_Code, market.MInstance, ShopControle);
                string fileGroupName = $"S_{market.Mcode}{ShopControle.City}{tableName}";

                // Check if the file group directory exists, and create it if not
                string marketDirectoryPath = Path.Combine($"{_path.MarketPath}", market.Mcode, $"{tableName}");
                string picDirectoryPath = Path.Combine($"{_path.MarketPath}", market.Mcode, $"{market.Mcode}{ShopControle.City}{tableName}", $"img{market.Mcode}{ShopControle.City}{tableName}");
                if (!Directory.Exists(marketDirectoryPath))
                {
                    Directory.CreateDirectory(marketDirectoryPath);
                    Directory.CreateDirectory(picDirectoryPath);
                }

                // Add the filegroup to the database if it doesn't exist
                string addFileGroupQuery = $@"
                    IF NOT EXISTS (SELECT * FROM sys.filegroups WHERE name = '{fileGroupName}')
                    BEGIN
                        ALTER DATABASE [{market.Mcode}]
                        ADD FILEGROUP [{fileGroupName}];

                        ALTER DATABASE [{market.Mcode}]
                        ADD FILE (
                            NAME = '{fileGroupName}_Filename',
                            FILENAME = '{Path.Combine(marketDirectoryPath, $"{fileGroupName}.ndf")}',
                            SIZE = 5MB,
                            MAXSIZE = 100MB,
                            FILEGROWTH = 5MB
                        ) TO FILEGROUP [{fileGroupName}];
                     END;
                            ";

                // Execute the filegroup creation query
                await dbContext.Database.ExecuteSqlRawAsync(addFileGroupQuery);
                string connectionString2 = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions2 = MarketDbContextFactory.CreateDbContextOptions(connectionString2);

                // Initialize the DbContext with the dynamic connection string
                using var dbContext2 = new MarketDbContext(dbContextOptions2);
                // Construct the table creation query, ensuring it is created within the file group
                string createTableQuery = $@"
                        CREATE TABLE [{fileGroupName}] (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            ProductName NVARCHAR(40) NOT NULL,
                            Description NVARCHAR(100),
                            Price DECIMAL(18, 2) NOT NULL,
                            DiscountedPrice DECIMAL(18, 2),
                            HasDiscount BIT,
                            TimeCreate DATETIME NOT NULL
                           
                        ) ON [{fileGroupName}];
                    ";

                // Execute the table creation command
                await dbContext2.Database.ExecuteSqlRawAsync(createTableQuery);

                return new GeneralRespond(true, "Table created successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"An error occurred while creating the product: {ex.Message}");
            }
        }

        public async Task UpdateVisitorCount(int shopId, string marketCode)
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

                // Table name for shop control
                var tableControl = $"C_{market.Mcode}";

                // SQL query to get the current NVistor value
                var query = $"SELECT [NVistor] FROM {tableControl} WHERE [Id] = @ShopId";

                using var connection = new SqlConnection(connectionString);
                var currentVisitor = await connection.QueryFirstOrDefaultAsync<int?>(query, new { ShopId = shopId });

                if (currentVisitor.HasValue)
                {
                    // Increment NVistor by 1
                    var newVisitorCount = currentVisitor.Value + 1;

                    // SQL query to update NVistor for the shop
                    var updateQuery = $"UPDATE {tableControl} SET [NVistor] = @NewVisitorCount WHERE [Id] = @ShopId";
                    await connection.ExecuteAsync(updateQuery, new { NewVisitorCount = newVisitorCount, ShopId = shopId });

                    Console.WriteLine($"Updated NVistor for ShopId {shopId} to {newVisitorCount}");
                }
                else
                {
                    Console.WriteLine($"Shop with Id {shopId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating NVistor: {ex.Message}");
            }
        }

        public async Task UpdateIsLockShope(int shopId, string marketCode, bool IsLock)
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

                // Table name for shop control
                var tableControl = $"C_{market.Mcode}";

                // SQL query to get the current NVistor value
                var query = $"SELECT [IsLock] FROM {tableControl} WHERE [Id] = @ShopId";

                using var connection = new SqlConnection(connectionString);
                var currentVisitor = await connection.QueryFirstOrDefaultAsync<int?>(query, new { ShopId = shopId });

                if (currentVisitor.HasValue)
                {
                    // Increment NVistor by 1
                    var IsLockShope = IsLock;

                    // SQL query to update NVistor for the shop
                    var updateQuery = $"UPDATE {tableControl} SET [IsLock] = @NewIsLock WHERE [Id] = @ShopId";
                    await connection.ExecuteAsync(updateQuery, new { NewIsLock = IsLockShope, ShopId = shopId });

                    Console.WriteLine($"Updated NVistor for ShopId {shopId} to {IsLockShope}");
                }
                else
                {
                    Console.WriteLine($"Shop with Id {shopId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating NVistor: {ex.Message}");
            }
        }
        public Task<GeneralRespond> DeleteAsync(int vendorId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShopControleResponseDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<ShopControle_LastCode_ResponseDTO> GetLastCode_ByCodeAsync(string m_Code)
        {
            try
            {
                // الحصول على معلومات السوق من قاعدة البيانات الرئيسية
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == m_Code);
                if (market == null)
                {
                    return null; // لم يتم العثور على السوق
                }

                // إعداد سلسلة الاتصال الديناميكية بناءً على `Instance` و`Shop_Code`
                string connectionString = $"Server={market.MInstance};Database={m_Code};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                using var dbContext = new MarketDbContext(dbContextOptions);

                // بناء اسم الجدول الديناميكي
                string tableName = $"C_{m_Code}";

                // البحث عن آخر كود `Last_VendorCode` في الجدول
                var lastVendorCodeRecord = await dbContext.Set<ShopControle_LastCode_ResponseDTO>()
                    .FromSqlRaw($"SELECT TOP 1  Last_VendorCode FROM [{tableName}] ORDER BY Id DESC")
                    .FirstOrDefaultAsync();

                string lastCode = lastVendorCodeRecord?.Last_VendorCode ?? "000";

                string newCode = CodeGenerator.GenerateNextCode(lastCode);

                // تحديث `Last_VendorCode` في قاعدة البيانات
                await dbContext.Database.ExecuteSqlRawAsync(
                    $"UPDATE [{tableName}] SET Last_VendorCode = @p0 WHERE Shop_Code = @p1",
                    newCode, m_Code
                );

                // إرجاع الكود الجديد كـ DTO
                return new ShopControle_LastCode_ResponseDTO
                {
                    Last_VendorCode = newCode
                };
            }
            catch (Exception ex)
            {
                // التعامل مع الخطأ في حالة الفشل
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


        // دالة لتحويل سلسلة Prim36 إلى عدد صحيح
        private int ConvertFromPrim36(string code)
        {
            const string primChars = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int result = 0;

            foreach (var c in code)
            {
                result = result * 36 + primChars.IndexOf(c);
            }
            return result;
        }

        // دالة لتحويل عدد صحيح إلى سلسلة من أساس Prim36
        private string ConvertToPrim36(int number, int length)
        {
            const string primChars = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = new Stack<char>();

            while (number > 0)
            {
                result.Push(primChars[number % 36]);
                number /= 36;
            }

            // التأكد أن النتيجة تحتوي على الطول المطلوب
            while (result.Count < length)
            {
                result.Push(primChars[0]); // إضافة الرقم الأدنى للحفاظ على طول السلسلة
            }

            return new string(result.ToArray());
        }


        public async Task<List<ShopControleResponseBYID_DTO>> GetVendorShopsAsync(int vendorId)
        {
            var result = new List<ShopControleResponseBYID_DTO>();

            // جلب جميع الأسواق من الجدول الرئيسي
            var markets = await _context.Markets.ToListAsync();

            foreach (var market in markets)
            {
                try
                {
                    // إعداد سلسلة الاتصال الديناميكية للداتابيس الخاصة بالسوق
                    string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                    var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                    using var dbContext = new MarketDbContext(dbContextOptions);

                    // اسم جدول التحكم للسوق الحالي
                    string tableName = $"C_{market.Mcode}";

                    // إعداد استعلام SQL لجلب بيانات المتجر المرتبطة بالبائع
                    var sqlQuery = $"SELECT Id, Name, City, Region, Street, NerestPoint FROM [{tableName}] WHERE Vendor_Id = @VendorId";
                    var vendorIdParam = new SqlParameter("@VendorId", vendorId);

                    // تنفيذ الاستعلام وإضافة النتائج إلى القائمة
                    var shopRecords = await dbContext.Set<ShopControleResponseBYID_DTO>()
                                                     .FromSqlRaw(sqlQuery, vendorIdParam)
                                                     .ToListAsync();

                    result.AddRange(shopRecords); // إضافة السجلات إلى القائمة النهائية
                }
                catch (Exception ex)
                {
                    // تسجيل الخطأ ومتابعة البحث في الأسواق الأخرى
                    Console.WriteLine($"Error accessing market {market.Mcode}: {ex.Message}");
                }
            }

            return result;
        }




        public async Task<GeneralRespond> UpdateAsync(ShopControleUpdateDTO ShopControleUpdate)
        {
            try
            {
                // البحث عن بيانات السوق واسم الجدول
                var market = await _context.Markets.FirstOrDefaultAsync(x => x.Mcode == ShopControleUpdate.Market_Code);
                if (market == null)
                    return new GeneralRespond(false, "Market not found.");

                // إنشاء سلسلة الاتصال للقاعدة الديناميكية
                string connectionString = $"Server={market.MInstance};Database={ShopControleUpdate.Market_Code};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                using var dbContext = new MarketDbContext(dbContextOptions);

                // بناء استعلام التحديث الديناميكي
                string tableName = $"C_{ShopControleUpdate.Market_Code}"; // اسم الجدول الديناميكي
                string updateCommand = $"UPDATE [{tableName}] SET Name = @Name, Region = @Region, Street = @Street, NerestPoint = @NerestPoint,StartWork=@StartWork,EndWork=@EndWork,Notes=@Notes WHERE Id = @Id";

                // تمرير القيم باستخدام SqlParameter
                var parameters = new[]
                {
                    new SqlParameter("@Name", ShopControleUpdate.Name),
                    new SqlParameter("@Region", ShopControleUpdate.Region),
                    new SqlParameter("@Street", ShopControleUpdate.Street),
                    new SqlParameter("@NerestPoint", ShopControleUpdate.NerestPoint),
                    new SqlParameter("@StartWork", (object?)ShopControleUpdate.StartWork?.ToTimeSpan() ?? DBNull.Value),
                    new SqlParameter("@EndWork",  (object?)ShopControleUpdate.EndWork?.ToTimeSpan() ?? DBNull.Value),
                    new SqlParameter("@Notes", ShopControleUpdate.Notes),
                    new SqlParameter("@Id", ShopControleUpdate.Id)
                 };

                // تنفيذ الأمر SQL
                int rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(updateCommand, parameters);

                // التحقق من نجاح التعديل
                if (rowsAffected > 0)
                    return new GeneralRespond(true, "تم تحديث السجل بنجاح.");
                else
                    return new GeneralRespond(false, "السجل غير موجود.");
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"حدث خطأ أثناء تحديث السجل: {ex.Message}");
            }
        }




        public async Task<ShopControleResponseBYID_DTO?> GetShopByIdAsync(int shopId, string marketCode)
        {
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.Mcode == marketCode);
            if (market == null)
            {
                throw new Exception("Market not found.");
            }

            // إعداد سلسلة الاتصال الديناميكية للسوق
            string connectionString = $"Server={market.MInstance};Database={market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // إعداد استعلام SQL لجلب بيانات المتجر بناءً على shopId
                string sqlQuery = $@"
            SELECT 
                Id, 
                Name, 
                Region, 
                Street, 
                NerestPoint, 
                StartWork, 
                EndWork, 
                Notes
            FROM C_{market.Mcode} 
            WHERE Id = @ShopId";

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@ShopId", shopId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // قراءة البيانات من SqlDataReader وتحويلها إلى DTO
                            var shop = new ShopControleResponseBYID_DTO
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Region = reader.GetString(reader.GetOrdinal("Region")),
                                Street = reader.GetString(reader.GetOrdinal("Street")),
                                NerestPoint = reader.GetString(reader.GetOrdinal("NerestPoint")),
                                StartWork = reader.IsDBNull(reader.GetOrdinal("StartWork"))
                                    ? (TimeOnly?)null
                                    : TimeOnly.FromDateTime(DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("StartWork")))),
                                EndWork = reader.IsDBNull(reader.GetOrdinal("EndWork"))
                                    ? (TimeOnly?)null
                                    : TimeOnly.FromDateTime(DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("EndWork")))),
                                Notes = reader.GetString(reader.GetOrdinal("Notes"))
                            };

                            return shop;
                        }
                        else
                        {
                            return null; // إذا لم يتم العثور على المتجر
                        }
                    }
                }
            }
        }









    }
}

