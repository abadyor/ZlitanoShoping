using APPL.Contract.Basket_ss;
using APPL.DTOs.Request.Basket_ss;
using APPL.DTOs.Response.Basket_ss;
using DL.Entities;
using INFL.Data;
using Microsoft.EntityFrameworkCore;

namespace INFL.Repositories.Basket_ss
{
    public class Basket_sRepo : IBasket_sRepo
    {
        private readonly ApplicationDbContext _context;

        public Basket_sRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        private Basket_sGetAllResponseDTO MapToGetAllResponseDTO(Basket_s basket_s)
        {
            return new Basket_sGetAllResponseDTO
            {
                Id = basket_s.Id,
                BasketId = basket_s.basketId,
                ItemId = basket_s.itemId,
                Quantity = basket_s.quantity,
                Totoal = basket_s.totoal,
                Date = basket_s.date
            };
        }

        private Basket_sGetByIdResponseDTO MapToGetByIdResponseDTO(Basket_s basket_s)
        {
            return new Basket_sGetByIdResponseDTO
            {
                Id = basket_s.Id,
                BasketId = basket_s.basketId,
                ItemId = basket_s.itemId,
                Quantity = basket_s.quantity,
                Totoal = basket_s.totoal,
                Date = basket_s.date
            };
        }

        private Basket_s MapToBasket_s(AddBasket_sRequestDTO dto)
        {
            return new Basket_s
            {
                basketId = dto.BasketId,
                itemId = dto.ItemId,
                quantity = dto.Quantity,
                totoal = dto.Totoal,
                date = dto.Date
            };
        }

        private void UpdateBasket_s(Basket_s basket_s, UpdateBasket_sRequestDTO dto)
        {
            basket_s.basketId = dto.BasketId;
            basket_s.itemId = dto.ItemId;
            basket_s.quantity = dto.Quantity;
            basket_s.totoal = dto.Totoal;

        }

        public async Task<IEnumerable<Basket_sGetAllResponseDTO>> GetAllBasket_sAsync()
        {
            var basket_sList = await _context.Basket_ss.ToListAsync();
            return basket_sList.Select(MapToGetAllResponseDTO);
        }

        public async Task<Basket_sGetByIdResponseDTO> GetBasket_sByIdAsync(int id)
        {
            var basket_s = await _context.Basket_ss.FindAsync(id);
            if (basket_s == null) return null;

            return MapToGetByIdResponseDTO(basket_s);
        }

        public async Task<List<Basket_sGetByIdResponseDTO>> GetBasket_sByBasketIdAsync(int basketId)
        {
            // جلب العناصر المرتبطة بالـ basketId
            var basket_s = await _context.Basket_ss
                                         .Where(b => b.basketId == basketId)
                                         .ToListAsync();

            // تحقق إذا كانت القائمة فارغة
            if (basket_s == null || !basket_s.Any())
                return null;

            // تحويل العناصر إلى DTO
            return basket_s.Select(MapToGetByIdResponseDTO).ToList();
        }


        public async Task<Basket_sGetByIdResponseDTO> AddBasket_sAsync(AddBasket_sRequestDTO dto)
        {
            var basket_s = MapToBasket_s(dto);
            _context.Basket_ss.Add(basket_s);
            await _context.SaveChangesAsync();
            return MapToGetByIdResponseDTO(basket_s);
        }

        public async Task<Basket_sGetByIdResponseDTO> UpdateBasket_sAsync(UpdateBasket_sRequestDTO dto)
        {
            var basket_s = await _context.Basket_ss.FindAsync(dto.Id);
            if (basket_s == null) return null;

            UpdateBasket_s(basket_s, dto);
            _context.Basket_ss.Update(basket_s);
            await _context.SaveChangesAsync();
            return MapToGetByIdResponseDTO(basket_s);
        }

        public async Task<bool> DeleteBasket_sAsync(int id)
        {
            var basket_s = await _context.Basket_ss.FindAsync(id);
            if (basket_s == null) return false;

            _context.Basket_ss.Remove(basket_s);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
