using APPL.Contract.Vendor;
using APPL.DTOs.Request.Vendor;
using APPL.DTOs.Response.Vendor;
using APPL.DTOS.Response;
using DL.Entities;
using INFL.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;


namespace INFL.Repositories.VendorRepo
{
    public class VendorRepo : IVendors
    {
        private readonly ApplicationDbContext _context;

        public VendorRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VendorResponseDTO> GetVendorByUserNamePasswordLogUser(string username, string password, string loguser)
        {
            var vendor = await _context.Vendors
                       .FirstOrDefaultAsync(v => v.Username == username &&
                         v.Password == password &&
                        v.Log_Code == loguser);

            var vendorResponseDTO = new VendorResponseDTO
            {
                Id = vendor.Id,
                GivenNames = vendor.GivenNames,
                Nickname = vendor.Nickname,
                Gender = vendor.Gender,
                DocId = vendor.DocId,
                DocType = vendor.DocType,
                EmailAddress = vendor.EmailAddress,
                Mobile = vendor.Mobile,
                Username = vendor.Username,
                TimestampCreate = vendor.Timestamp_create
            };

            return vendorResponseDTO;
        }

        public async Task<GeneralRespond> CreateAsync(VendorRequestCreateDTO vendorRequestCreateDTO)
        {
            try
            {
                var vendor = new Vendor
                {
                    GivenNames = vendorRequestCreateDTO.GivenNames,
                    Nickname = vendorRequestCreateDTO.Nickname,
                    Gender = vendorRequestCreateDTO.Gender,
                    DocId = vendorRequestCreateDTO.DocId,
                    DocType = vendorRequestCreateDTO.DocType,
                    EmailAddress = vendorRequestCreateDTO.EmailAddress,
                    Mobile = vendorRequestCreateDTO.Mobile,
                    Username = vendorRequestCreateDTO.Username,
                    Password = vendorRequestCreateDTO.Password,
                    Timestamp_create = DateTime.UtcNow
                };

                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();

                var code = await GenerateCodeAndSendEmail(vendorRequestCreateDTO.EmailAddress);

                var x = await updateonefild(code);
                return new GeneralRespond(true, "Vendor created successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"Error creating vendor: {ex.Message}");
            }
        }

        public async Task<GeneralRespond> DeleteAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null)
                return new GeneralRespond(false, "Vendor not found.");

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return new GeneralRespond(true, "Vendor deleted successfully.");
        }

        public async Task<List<VendorResponseDTO>> GetAllAsync()
        {
            return await _context.Vendors
                .Select(v => new VendorResponseDTO
                {
                    Id = v.Id,
                    GivenNames = v.GivenNames,
                    Nickname = v.Nickname,
                    Gender = v.Gender,
                    DocId = v.DocId,
                    DocType = v.DocType,
                    EmailAddress = v.EmailAddress,
                    Mobile = v.Mobile,
                    Username = v.Username,
                    TimestampCreate = v.Timestamp_create
                })
                .ToListAsync();
        }
        public async Task<List<VendorResponseDTO>> GetAllStoreCodesAsync()
        {
            return await _context.Vendors
                .Select(v => new VendorResponseDTO
                {
                    Id = v.Id,
                    Username = v.Username
                })
                .ToListAsync();
        }

        public async Task<VendorResponse_Stores_DTO> GetByIdAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null) return null;

            return new VendorResponse_Stores_DTO
            {
                Id = vendor.Id,
                GivenNames = vendor.GivenNames,

            };
        }

        public async Task<VendorResponseDTO?> VendorUsernamePasswordAsync(string username, string password)
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Username == username && v.Password == password);

            if (vendor == null)
            {
                return null; // يمكن استبدال هذا بإرجاع كائن يحمل رسالة خطأ إذا كنت ترغب.
            }

            return new VendorResponseDTO
            {
                Id = vendor.Id,
                GivenNames = vendor.GivenNames,
                Nickname = vendor.Nickname,
                Gender = vendor.Gender,
                DocId = vendor.DocId,
                DocType = vendor.DocType,
                EmailAddress = vendor.EmailAddress,
                Mobile = vendor.Mobile,
                Username = vendor.Username,
                Password = vendor.Password,
                TimestampCreate = vendor.Timestamp_create
            };
        }

        public async Task<GeneralRespond> VendorLogCode_ChekAsync(string username, string logCode)
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Username == username && v.Log_Code == logCode);

            if (vendor == null)
                return new GeneralRespond(false, "Invalid log code.");

            return new GeneralRespond(true, "Log code verified.");
        }

        public async Task<GeneralRespond> UpdateLogCodeAsync(VendorLogCode_UpdateDTO vendorLogCode_UpdateDTO)
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Username == vendorLogCode_UpdateDTO.Username);

            if (vendor == null)
                return new GeneralRespond(false, "Vendor not found.");

            vendor.Log_Code = vendorLogCode_UpdateDTO.Log_Code;
            await _context.SaveChangesAsync();
            return new GeneralRespond(true, "Log code updated successfully.");
        }

        public Task<VendorResponse_Stores_DTO> GetByCodeAsync(string vendorCode)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralRespond> VendorLoginAsync(VendorLoginDTO loginDto)
        {
            try
            {
                // البحث عن البائع في قاعدة البيانات باستخدام اسم المستخدم
                var _vendor = await _context.Vendors
                    .FirstOrDefaultAsync(m => m.Username == loginDto.Username);

                if (_vendor == null)
                    return new GeneralRespond(false, "البائع غير موجود.");

                // التحقق من كلمة المرور
                if (_vendor.Password != loginDto.Password)
                    return new GeneralRespond(false, "كلمة المرور غير صحيحة.");

                // توليد كود مكون من 6 أرقام
                var logCode = new Random().Next(100000, 999999).ToString();

                // تحديث الكود في حقل Log_Code
                _vendor.Log_Code = logCode;
                await _context.SaveChangesAsync();

                // إرسال الكود إلى البريد الإلكتروني للبائع
                await SendEmailAsync(_vendor.EmailAddress, "رمز تأكيد الهوية", $"رمز تأكيد الهوية الخاص بك هو: {logCode}");

                return new GeneralRespond(true, "تم تسجيل الدخول بنجاح، وتم إرسال رمز التحقق إلى البريد الإلكتروني.");
            }
            catch (Exception ex)
            {
                return new GeneralRespond(false, $"حدث خطأ أثناء عملية تسجيل الدخول: {ex.Message}");
            }
        }



        public async Task<string> GenerateCodeAndSendEmailAndGetCode(string recipientEmail)
        {
            // توليد كود عشوائي
            string code = GenerateRandomCode();

            // إرسال البريد الإلكتروني
            await SendEmailAsync1(recipientEmail, code);


            return "OkSend"; // يمكن إرجاع الكود إذا لزم الأمر
        }

        public async Task<string> GenerateCodeAndSendEmail(string recipientEmail)
        {
            // توليد كود عشوائي
            string code = GenerateRandomCode();

            // إرسال البريد الإلكتروني
            await SendEmailAsync1(recipientEmail, code);
            // codegeny = code;

            return code; // يمكن إرجاع الكود إذا لزم الأمر
        }
        private string GenerateRandomCode()
        {
            // توليد كود عشوائي مكون من 8 أرقام (يمكنك تعديله حسب الحاجة)
            Random random = new Random();
            return random.Next(10000000, 99999999).ToString("D8"); // كود مكون من 8 أرقام
        }

        private async Task SendEmailAsync1(string recipientEmail, string code)
        {
            try
            {
                // **Security:** Use a more secure authentication method (recommended)
                // Consider using OAuth 2.0 or a third-party email service provider

                // **If using App Password (less secure):**
                var fromAddress = new MailAddress("abdosend873@gmail.com", "مرحبا بك شركة زليتانو"); // Replace with App Password
                var toAddress = new MailAddress(recipientEmail);
                const string subject = "كود التفعيل";
                string body = $"Your verification code is: {code}";

                using (var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromAddress.Address,
         "xcvjulophxvuhxuk") // Use address for username
                })
                {
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    })
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // **Improved Logging:** Use a logging library (e.g., NLog, Serilog)
                Console.WriteLine($"Error sending email: {ex.Message}"); // Basic logging for now
            }
        }
        private async Task<int> GetLastVendorLastId()
        {
            var lastId = await _context.Vendors
                .OrderByDescending(v => v.Id)
                .FirstOrDefaultAsync();
            return lastId?.Id ?? 0;
        }

        public async Task<string> updateonefild(string code)
        {
            var lastid = await GetLastVendorLastId();
            if (lastid == 0)
            {
                return "The Value 0";
            }
            else
            {
                var entity = await _context.Vendors.FindAsync(lastid);
                if (entity == null)
                {
                    throw new Exception("Entity not found");
                }

                entity.Log_Code = code;
                await _context.SaveChangesAsync();

                return "The Value Updated";
            }
        }

        // دالة مساعدة لإرسال البريد الإلكتروني
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // إعدادات خدمة البريد الإلكتروني
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("ramadan.a.daadosh@gmail.com", "Remo@1234ahmed"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("ramadan.a.daadosh@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }


    }

}
