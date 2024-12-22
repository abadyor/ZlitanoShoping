using APPL.DTOs.Request.Basket_ss;
using APPL.DTOs.Response.Basket_ss;

namespace APPL.Contract.Basket_ss
{

    public interface IBasket_sRepo
    {
        Task<IEnumerable<Basket_sGetAllResponseDTO>> GetAllBasket_sAsync();
        Task<Basket_sGetByIdResponseDTO> GetBasket_sByIdAsync(int id);
        Task<Basket_sGetByIdResponseDTO> AddBasket_sAsync(AddBasket_sRequestDTO dto);
        Task<Basket_sGetByIdResponseDTO> UpdateBasket_sAsync(UpdateBasket_sRequestDTO dto);
        Task<bool> DeleteBasket_sAsync(int id);

        public Task<List<Basket_sGetByIdResponseDTO>> GetBasket_sByBasketIdAsync(int basketId);
    }
}
