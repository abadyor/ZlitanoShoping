using APPL.DTOs.Request.Baskets;
using APPL.DTOs.Response.Baskets;

namespace APPL.Contract.Baskets
{
    public interface IBasketRepo
    {
        Task<IEnumerable<BasketGetAllResponseDTO>> GetAllBasketsAsync();
        Task<BasketGetByIdResponseDTO> GetBasketByIdAsync(int id);
        Task<BasketGetByIdResponseDTO> AddBasketAsync(AddBasketRequsetDTO basketDto);
        Task<BasketGetByIdResponseDTO> UpdateBasketAsync(UpdateBasketRequestDTO basketDto);
        Task<bool> DeleteBasketAsync(int id);
        Task<IEnumerable<BasketGetAllResponseDTO>> GetBasketsByCustomerIdAsync(int customerId);
        public Task<string> GenerateCodeAndSendEmail(string recipientEmail);
        public Task<string> updateonefild(string code, int basketId);
        public Task<string> GetCustomerandSendEmailAndupdte(int customerId, int BasketId);
        public Task<BasketGetAllResponseDTO> GetBasketBybasketIdandCustomerIDLogUser(int basketId, int customerId, string loguser);
        public Task<string> updateCloseBasket(int basketId, bool value);
    }
}
