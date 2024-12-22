using APPL.Contract.Baskets;
using APPL.Contract.Customers;
using APPL.DTOs.Request.Baskets;
using APPL.DTOs.Response.Baskets;
using DL.Entities;
using INFL.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace INFL.Repositories.Baskets
{
    public class BasketRepo : IBasketRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomer _customer;

        public BasketRepo(ApplicationDbContext context, ICustomer customer)
        {
            _context = context;
            _customer = customer;
        }

        // Helper Methods for Mapping
        private BasketGetAllResponseDTO MapToBasketGetAllResponseDTO(Basket basket)
        {
            return new BasketGetAllResponseDTO
            {
                Id = basket.Id,
                customerId = basket.customerId,
                nameIdShope = basket.nameIdShope,
                Date = basket.Date,
                tootal = basket.tootal,
                countElementBasket = basket.countElementBasket,
                codeBasket = basket.codeBasket,
                closeBasket = basket.closeBasket,

            };
        }

        private BasketGetByIdResponseDTO MapToBasketGetByIdResponseDTO(Basket basket)
        {
            return new BasketGetByIdResponseDTO
            {
                Id = basket.Id,
                customerId = basket.customerId,
                nameIdShope = basket.nameIdShope,
                Date = basket.Date,
                tootal = basket.tootal,
                countElementBasket = basket.countElementBasket,
                codeBasket = basket.codeBasket,
                closeBasket = basket.closeBasket,
                loguser = basket.loguser
            };
        }

        private Basket MapToBasket(AddBasketRequsetDTO dto)
        {
            return new Basket
            {
                customerId = dto.customerId,
                nameIdShope = dto.nameIdShope,
                Date = DateTime.Now,
                tootal = dto.tootal,
                countElementBasket = dto.countElementBasket,
                codeBasket = dto.nameIdShope + dto.customerId,

            };
        }

        private Basket MapToBasket(UpdateBasketRequestDTO dto)
        {
            return new Basket
            {
                Id = dto.Id,
                customerId = dto.customerId,
                nameIdShope = dto.nameIdShope,
                Date = dto.Date,
                tootal = dto.tootal,
                countElementBasket = dto.countElementBasket,
                codeBasket = dto.codeBasket,
                closeBasket = dto.closeBasket,
                loguser = dto.loguser
            };
        }

        // Methods
        public async Task<IEnumerable<BasketGetAllResponseDTO>> GetAllBasketsAsync()
        {
            var baskets = await _context.Baskets.Include(b => b.customer).ToListAsync();
            return baskets.Select(MapToBasketGetAllResponseDTO);
        }

        public async Task<BasketGetByIdResponseDTO> GetBasketByIdAsync(int id)
        {
            var basket = await _context.Baskets.Include(b => b.customer).FirstOrDefaultAsync(b => b.Id == id);
            if (basket == null)
                return null;

            return MapToBasketGetByIdResponseDTO(basket);
        }

        public async Task<BasketGetByIdResponseDTO> AddBasketAsync(AddBasketRequsetDTO dto)
        {
            var basket = MapToBasket(dto);
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();

            return MapToBasketGetByIdResponseDTO(basket);
        }

        public async Task<BasketGetByIdResponseDTO> UpdateBasketAsync(UpdateBasketRequestDTO dto)
        {
            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.Id == dto.Id);
            if (basket == null)
                return null;

            basket.customerId = dto.customerId;
            basket.nameIdShope = dto.nameIdShope;
            basket.Date = dto.Date;
            basket.tootal = dto.tootal;
            basket.countElementBasket = dto.countElementBasket;
            basket.codeBasket = dto.codeBasket;
            basket.closeBasket = dto.closeBasket;
            basket.loguser = dto.loguser;

            _context.Baskets.Update(basket);
            await _context.SaveChangesAsync();

            return MapToBasketGetByIdResponseDTO(basket);
        }

        public async Task<bool> DeleteBasketAsync(int id)
        {
            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.Id == id);
            if (basket == null)
                return false;

            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BasketGetAllResponseDTO>> GetBasketsByCustomerIdAsync(int customerId)
        {
            var baskets = await _context.Baskets.Where(b => b.customerId == customerId && b.closeBasket == false).ToListAsync();
            return baskets.Select(MapToBasketGetAllResponseDTO);
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

        public async Task<string> updateCloseBasket(int basketId, bool value)
        {
            var lastid = basketId;
            if (lastid == 0)
            {
                return "The Value 0";
            }
            else
            {
                var entity = await _context.Baskets.FindAsync(lastid);
                if (entity == null)
                {
                    throw new Exception("Entity not found");
                }

                entity.closeBasket = value;
                await _context.SaveChangesAsync();

                return "The Value Updated";
            }
        }

        public async Task<string> updateonefild(string code, int basketId)
        {
            var lastid = basketId;
            if (lastid == 0)
            {
                return "The Value 0";
            }
            else
            {
                var entity = await _context.Baskets.FindAsync(lastid);
                if (entity == null)
                {
                    throw new Exception("Entity not found");
                }

                entity.loguser = code;
                await _context.SaveChangesAsync();

                return "The Value Updated";
            }
        }

        public async Task<string> GetCustomerandSendEmailAndupdte(int customerId, int BasketId)
        {
            var cust = await _customer.GetByIdAsync(customerId);
            if (cust == null)
            {
                return "no";
            }


            var code = await GenerateCodeAndSendEmail(cust.EmailAddress);
            var updatefield = await updateonefild(code, BasketId);
            return "ok Compleate";


        }



        public async Task<BasketGetAllResponseDTO> GetBasketBybasketIdandCustomerIDLogUser(int basketId, int customerId, string loguser)
        {
            var vendor = await _context.Baskets
                       .FirstOrDefaultAsync(v => v.Id == basketId &&
                         v.customerId == customerId &&
                        v.loguser == loguser);

            var customerResponseDTO = new BasketGetAllResponseDTO
            {
                Id = vendor.Id,
                customerId = vendor.customerId,
                nameIdShope = vendor.nameIdShope,
                Date = vendor.Date,
                tootal = vendor.tootal,
                countElementBasket = vendor.countElementBasket,
                codeBasket = vendor.codeBasket,
                closeBasket = vendor.closeBasket,

            };

            return customerResponseDTO;
        }
    }
}
