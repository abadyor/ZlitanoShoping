using APPL.Contract.Customers;
using APPL.DTOs.Request.Customers;
using APPL.DTOs.Response.Customers;
using DL.Entities;
using INFL.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace INFL.Repositories.Customers
{
    public class CustomerRepo : ICustomer
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<CustomerGetAllResponseDTO> GetVendorByUserNamePasswordLogUser(string username, string password, string loguser)
        {
            var vendor = await _context.Customers
                       .FirstOrDefaultAsync(v => v.Username == username &&
                         v.Password == password &&
                        v.loguser == loguser);

            var customerResponseDTO = new CustomerGetAllResponseDTO
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
                Timestamp_create = vendor.Timestamp_create
            };

            return customerResponseDTO;
        }

        public async Task<List<CustomerGetAllResponseDTO>> GetAllAsync()
        {
            return await _context.Customers
                .Select(c => new CustomerGetAllResponseDTO
                {
                    Id = c.Id,
                    GivenNames = c.GivenNames,
                    Nickname = c.Nickname,
                    EmailAddress = c.EmailAddress,
                    Mobile = c.Mobile,
                    TootalPurchases = c.TootalPurchases
                })
                .ToListAsync();
        }

        public async Task<CustomerGetByIdResponseDTO> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return null;

            return new CustomerGetByIdResponseDTO
            {
                Id = customer.Id,
                GivenNames = customer.GivenNames,
                Nickname = customer.Nickname,
                Gender = customer.Gender,
                DocId = customer.DocId,
                DocType = customer.DocType,
                EmailAddress = customer.EmailAddress,
                Mobile = customer.Mobile,
                Timestamp_create = customer.Timestamp_create,
                CountBasket = customer.CountBasket,
                TootalPurchases = customer.TootalPurchases
            };
        }
        public async Task<CustomerGetAllResponseDTO?> CustomerUsernamePasswordAsync(string username, string password)
        {
            var cusotmer = await _context.Customers
                .FirstOrDefaultAsync(v => v.Username == username && v.Password == password);

            if (cusotmer == null)
            {
                return null; // يمكن استبدال هذا بإرجاع كائن يحمل رسالة خطأ إذا كنت ترغب.
            }

            return new CustomerGetAllResponseDTO
            {
                Id = cusotmer.Id,
                GivenNames = cusotmer.GivenNames,
                Nickname = cusotmer.Nickname,
                Gender = cusotmer.Gender,
                DocId = cusotmer.DocId,
                DocType = cusotmer.DocType,
                EmailAddress = cusotmer.EmailAddress,
                Mobile = cusotmer.Mobile,
                Username = cusotmer.Username,
                Password = cusotmer.Password,
                Timestamp_create = cusotmer.Timestamp_create
            };
        }

        private async Task<int> GetLastVendorLastId()
        {
            var lastId = await _context.Customers
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
                var entity = await _context.Customers.FindAsync(lastid);
                if (entity == null)
                {
                    throw new Exception("Entity not found");
                }

                entity.loguser = code;
                await _context.SaveChangesAsync();

                return "The Value Updated";
            }
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
         "fduvduibgronztvz") // Use address for username
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
        public async Task AddAsync(CustomerAddRequestDTO customerDto)
        {
            var customer = new Customer
            {
                GivenNames = customerDto.GivenNames,
                Nickname = customerDto.Nickname,
                Gender = customerDto.Gender,
                DocId = customerDto.DocId,
                DocType = customerDto.DocType,
                EmailAddress = customerDto.EmailAddress,
                Mobile = customerDto.Mobile,
                Username = customerDto.Username,
                Password = customerDto.Password,
                Timestamp_create = DateTime.Now
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            var code = await GenerateCodeAndSendEmail(customerDto.EmailAddress);

            var x = await updateonefild(code);
        }

        public async Task UpdateAsync(CustomerUpdateRequestDTO customerDto)
        {
            var customer = await _context.Customers.FindAsync(customerDto.Id);
            if (customer == null) throw new Exception("Customer not found.");

            customer.GivenNames = customerDto.GivenNames ?? customer.GivenNames;
            customer.Nickname = customerDto.Nickname ?? customer.Nickname;
            customer.Gender = customerDto.Gender ?? customer.Gender;
            customer.DocId = customerDto.DocId ?? customer.DocId;
            customer.DocType = customerDto.DocType ?? customer.DocType;
            customer.EmailAddress = customerDto.EmailAddress ?? customer.EmailAddress;
            customer.Mobile = customerDto.Mobile ?? customer.Mobile;
            customer.Username = customerDto.Username ?? customer.Username;
            customer.Password = customerDto.Password ?? customer.Password;


            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) throw new Exception("Customer not found.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CustomerExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}
