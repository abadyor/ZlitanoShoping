using APPL.DTOs.Request.Market;
using APPL.DTOs.Request.ShopCntrole;
using APPL.DTOs.Response.Market;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOS.Response;


namespace APPL.Contract.Market
{
    public interface IMarekrt
    {
        Task<GeneralRespond> AddMarketAsync(MarketTBCreatetDTO request);
        Task<GeneralRespond> CreateMarketControlAsync(string request);
        Task<GeneralRespond> UpdateAsync(ShopControleUpdateDTO ShopControleUpdate);
        Task<List<ShopControleResponseBYID_DTO>> GetVendorShopsAsync(int vendorId);
        Task<ShopControle_LastCode_ResponseDTO> GetByIdAsync(string Shop_Code);
        Task<List<MarketGetAllResponseDTO>> GetAllAsyncData();


    }
}
