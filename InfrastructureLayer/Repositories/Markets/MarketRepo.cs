using APPL.Contract.Market;
using APPL.DTOs.Request.Market;
using APPL.DTOs.Request.ShopCntrole;
using APPL.DTOs.Response.Market;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOS.Response;
using DL.Entities;
using INFL.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace INFL.Repositories.Markets
{
    public class MarketRepo : IMarekrt
    {
        private readonly ApplicationDbContext _mainDbContext;

        private readonly PathSettings _path;
        public MarketRepo(ApplicationDbContext mainDbContext, IOptions<PathSettings> path)
        {
            _mainDbContext = mainDbContext;
            _path = path.Value;
        }
        public string GetLastCode()
        {
            // الحصول على آخر كود مسجل بناءً على الترتيب التنازلي لحقل Scode
            var lastCode = _mainDbContext.Markets
                                   .OrderByDescending(s => s.Mcode)
                                   .Select(s => s.Mcode)
                                   .FirstOrDefault();

            // إذا لم يتم العثور على أي كود، ارجع "001" كبداية
            return lastCode ?? "000";
        }
        /*  public async Task<GeneralRespond> AddMarketAsync(MarketTBCreatetDTO request)
          {
              try
              {
                  string lastCode = GetLastCode();
                  string newCode = CodeGenerator.GenerateNextCode(lastCode);
                  // إضافة بيانات السوق في قاعدة البيانات الرئيسية
                  var market = new Market
                  {
                      Name = request.Name,
                      Mcode = newCode,
                      MInstance = request.SInstance
                  };
                  _mainDbContext.Markets.Add(market);
                  await _mainDbContext.SaveChangesAsync();

                  // إعداد سلسلة الاتصال للدخول إلى قاعدة بيانات النظام الرئيسي
                  string connectionString = $"Server={request.SInstance};Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
                  var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                  using var dbContext = new MarketDbContext(dbContextOptions);

                  // إعداد مسار الملف لمجلد السوق
                  //  string marketDirectoryPath = Path.Combine("F:\\Databases", newCode);
                  string marketDirectoryPath = Path.Combine($"{_path.MarketPath}", newCode);
                  string filePath = Path.Combine(marketDirectoryPath, $"D_{newCode}.ndf");

                  // التأكد من إنشاء المجلدات إذا لم تكن موجودة
                  if (!Directory.Exists(marketDirectoryPath))
                  {
                      Directory.CreateDirectory(marketDirectoryPath);
                  }

                  // إنشاء قاعدة البيانات يدويًا باستخدام SQL
                  string createDatabaseSql = $@"
              CREATE DATABASE [{newCode}]
              ON PRIMARY (
                  NAME = N'{newCode}_Primary',
                  FILENAME = '{filePath}',
                  SIZE = 5MB,
                  MAXSIZE = 100MB,
                  FILEGROWTH = 5MB
              )
              LOG ON (
                  NAME = N'{newCode}_Log',
                  FILENAME = '{Path.Combine(marketDirectoryPath, $"D_{newCode}_Log.ldf")}',
                  SIZE = 1MB,
                  MAXSIZE = 50MB,
                  FILEGROWTH = 1MB
              )";

                  await dbContext.Database.ExecuteSqlRawAsync(createDatabaseSql);

                  // في حالة نجاح العملية
                  return new GeneralRespond(true, "تم إنشاء السوق وقاعدة البيانات بنجاح.");
              }
              catch (SqlException sqlEx)
              {
                  // التعامل مع أخطاء SQL بشكل محدد
                  return new GeneralRespond(false, $"خطأ في SQL أثناء إنشاء السوق: {sqlEx.Message}");
              }
              catch (Exception ex)
              {
                  // في حالة حدوث خطأ آخر
                  return new GeneralRespond(false, $"حدث خطأ أثناء إنشاء السوق: {ex.Message}");
              }
          }*/

        public async Task<GeneralRespond> AddMarketAsync(MarketTBCreatetDTO request)
        {
            try
            {
                // توليد الكود الجديد للسوق
                string lastCode = GetLastCode();
                string newCode = CodeGenerator.GenerateNextCode(lastCode);

                // إضافة بيانات السوق في قاعدة البيانات الرئيسية
                var market = new Market
                {
                    Name = request.Name,
                    Mcode = newCode,
                    MInstance = request.SInstance
                };

                // إضافة السوق إلى قاعدة البيانات
                _mainDbContext.Markets.Add(market);
                await _mainDbContext.SaveChangesAsync();

                // إعداد سلسلة الاتصال للدخول إلى قاعدة بيانات النظام الرئيسي
                string connectionString = $"Server={request.SInstance};Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);

                using var dbContext = new MarketDbContext(dbContextOptions);

                // إعداد مسار الملف لمجلد السوق
                string marketDirectoryPath = Path.Combine($"{_path.MarketPath}", newCode);
                string filePath = Path.Combine(marketDirectoryPath, $"D_{newCode}.ndf");

                // التأكد من إنشاء المجلدات إذا لم تكن موجودة
                if (!Directory.Exists(marketDirectoryPath))
                {
                    Directory.CreateDirectory(marketDirectoryPath);
                }

                // إنشاء قاعدة البيانات يدويًا باستخدام SQL
                string createDatabaseSql = $@"
        CREATE DATABASE [{newCode}]
        ON PRIMARY (
            NAME = N'{newCode}_Primary',
            FILENAME = '{filePath}',
            SIZE = 5MB,
            MAXSIZE = 100MB,
            FILEGROWTH = 5MB
        )
        LOG ON (
            NAME = N'{newCode}_Log',
            FILENAME = '{Path.Combine(marketDirectoryPath, $"D_{newCode}_Log.ldf")}',
            SIZE = 1MB,
            MAXSIZE = 50MB,
            FILEGROWTH = 1MB
        )";

                await dbContext.Database.ExecuteSqlRawAsync(createDatabaseSql);

                // بعد إضافة السوق بنجاح، استدعاء دالة CreateMarketControlAsync
                var createControlResult = await CreateMarketControlAsync(newCode);

                if (createControlResult.Flag)
                {
                    // إذا كانت الدالة CreateMarketControlAsync ناجحة
                    return new GeneralRespond(true, "تم إنشاء السوق وقاعدة البيانات بنجاح، وتم إنشاء تحكم السوق بنجاح.");
                }
                else
                {
                    // إذا فشلت دالة CreateMarketControlAsync
                    return new GeneralRespond(false, $"تم إنشاء السوق بنجاح ولكن فشل إنشاء تحكم السوق: {createControlResult.Message}");
                }
            }
            catch (SqlException sqlEx)
            {
                // التعامل مع أخطاء SQL بشكل محدد
                return new GeneralRespond(false, $"خطأ في SQL أثناء إنشاء السوق: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // في حالة حدوث خطأ آخر
                return new GeneralRespond(false, $"حدث خطأ أثناء إنشاء السوق: {ex.Message}");
            }
        }
        public async Task<GeneralRespond> CreateMarketControlAsync(string request)
        {
            try
            {
                var _Market = await _mainDbContext.Markets.FirstOrDefaultAsync(m => m.Mcode == request);
                if (_Market == null)
                    return new GeneralRespond(false, "_Market not found.");


                string connectionString = $"Server={_Market.MInstance};Database={_Market.Mcode};Trusted_Connection=True;TrustServerCertificate=True;";
                var dbContextOptions = MarketDbContextFactory.CreateDbContextOptions(connectionString);
                using var dbContext = new MarketDbContext(dbContextOptions);

                // Define directory path and ensure it exists
                string marketDirectoryPath = Path.Combine($"{_path.MarketPath}", _Market.Mcode, $"C_{_Market.Mcode}");
                if (!Directory.Exists(marketDirectoryPath))
                {
                    Directory.CreateDirectory(marketDirectoryPath);
                }

                // Define the filegroup name
                string fileGroupName = $"C_{_Market.Mcode}";

                // Check if the filegroup exists, and create it if it does not
                string createFileGroupQuery = $@"
            IF NOT EXISTS (SELECT * FROM sys.filegroups WHERE name = '{fileGroupName}')
            BEGIN
                ALTER DATABASE [{_Market.Mcode}]
                ADD FILEGROUP [{fileGroupName}];
                
                ALTER DATABASE [{_Market.Mcode}]
                ADD FILE (NAME = N'{fileGroupName}_File', FILENAME = N'{Path.Combine(marketDirectoryPath, $"{fileGroupName}.ndf")}')
                TO FILEGROUP [{fileGroupName}];
            END
        ";

                await dbContext.Database.ExecuteSqlRawAsync(createFileGroupQuery);

                // Define and create the table on the specified filegroup
                string createTableQuery = $@"
            CREATE TABLE [{_Market.Mcode}].dbo.[C_{_Market.Mcode}](
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(50) NOT NULL,
                Market_Code NVARCHAR(8) NOT NULL,
                Last_VendorCode NVARCHAR(8) NOT NULL,
                City NVARCHAR(8),
                Region NVARCHAR(50),
                Street NVARCHAR(50),
                NerestPoint NVARCHAR(50),
                [NVistor] [bigint] NOT NULL CONSTRAINT [DF_C_001_NVistor]  DEFAULT ((0)),
                Vendor_Id INT,
                IsLock BIT  DEFAULT 0,
                StartWork TIME ,
                EndWork TIME ,
                Notes NVARCHAR(100),
                Priority INT DEFAULT 0
            ) ON [{fileGroupName}];
        ";

                await dbContext.Database.ExecuteSqlRawAsync(createTableQuery);


                string insertQuery = $"INSERT INTO [{fileGroupName}] (Name, Market_Code, Last_VendorCode, City) " +
                       "VALUES (@Name, @Market_Code, @Last_VendorCode, @City)";

                var parameters = new[]
                {
        new SqlParameter("@Name", _Market.Name),
        new SqlParameter("@Market_Code", _Market.Mcode),
        new SqlParameter("@Last_VendorCode", "000"), // استخدام الكود الجديد
        new SqlParameter("@City", "00")

    };

                // تنفيذ استعلام الإدخال لإضافة السجل الجديد
                await dbContext.Database.ExecuteSqlRawAsync(insertQuery, parameters);

                return new GeneralRespond(true, "Market control table created successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"An error occurred: {ex.Message}");
            }
        }

        public Task<ShopControle_LastCode_ResponseDTO> GetByIdAsync(string Shop_Code)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShopControleResponseBYID_DTO>> GetVendorShopsAsync(int vendorId)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralRespond> UpdateAsync(ShopControleUpdateDTO ShopControleUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketGetAllResponseDTO>> GetAllAsyncData()
        {
            var x = await _mainDbContext.Markets.ToListAsync();
            var marketDTOs = x.Select(market => new MarketGetAllResponseDTO
            {
                Id = market.Id,
                Name = market.Name,
                Mcode = market.Mcode,
                MInstance = market.MInstance,
                IsLock = market.IsLock
            }).ToList();

            return marketDTOs;
        }
    }
}
