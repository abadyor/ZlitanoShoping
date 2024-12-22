using APPL.DTOs.Request.ShopCntrole;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOS.Response;

namespace APPL.Contract.MarketControl
{
    public interface IMarketControl
    {
        Task<GeneralRespond> CreateAsync(ShopControleCreateDTO ShopControle);
        Task<GeneralRespond> DeleteAsync(int vendorId);
        Task<List<ShopControleResponseDTO>> GetAllAsync();
        Task<ShopControle_LastCode_ResponseDTO> GetLastCode_ByCodeAsync(string Shop_Code);
        Task<GeneralRespond> UpdateAsync(ShopControleUpdateDTO ShopControleUpdate);
        Task<List<ShopControleResponseBYID_DTO>> GetVendorShopsAsync(int vendorId);

        public Task UpdateVisitorCount(int shopId, string marketCode);
        public Task UpdateIsLockShope(int shopId, string marketCode, bool IsLock);
        public Task<ShopControleResponseBYID_DTO?> GetShopByIdAsync(int shopId, string marketCode);
    }
}
