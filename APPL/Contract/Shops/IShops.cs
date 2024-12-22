using APPL.DTOs.Request.Shops;
using APPL.DTOs.Response.ShopControle;
using APPL.DTOs.Response.Shops;
using APPL.DTOS.Response;
using Microsoft.AspNetCore.Http;

namespace APPL.Contract.Shops
{
    public interface IShops
    {
        Task<IEnumerable<ShopsResponseDTO>> GetAllAsync();
        Task<ShopsResponseDTO> GetByIdAsync(int id);
        //Task<ShopsResponseDTO> GetShopeByIdAsync(int id);
        Task<int> CreateAsync(ShopsCreateDTO createDto);
        Task<GeneralRespond> UpdateAsync(ShopsUpdateDTO updateDto);
        Task<GeneralRespond> UpdateLockAsync(ShopsUpdateLockDTO shopsUpdateLockDTO);
        Task<GeneralRespond> DeleteAsync(int id);
        Task<string> UploadImageAsync(IFormFile file, string folder, int id);
        Task<List<ShopeControllResponseDTO>> GetAllShope(string marketCode);
        //public Task<List<ShopeControllResponseDTO>> GetShopeByVendorAcrossAllMarkets(int vendorId);
        public Task<List<ShopeControllWithMarketNameResponseDTO>> GetShopeByVendorAcrossAllMarkets(int vendorId);
        //public Task<Dictionary<int, ItemDataWithImagesDTO>> GetItemsDataWithImages(string tablename);
        public Task<List<ItemDataWithImagesDTO>> GetItemsDataWithImages(string tableName);
        public Task<List<ItemDataWithImagesDTO>> GetItemsDataWithImagesWhereId(string tableName, int id);
        public Task UpdateItemData(string tableName, UpdateShopeDTO updatedItem);

        public Task<string> UpdateImageAsync(IFormFile file, string folder, int id, string nameold);
        void DeleteImage(string folder, string nameold);
        public Task<List<ShopeControllGetAllWithImageDTO>> GetAllShope1(string marketCode);
        public Task<List<ShopeControllGetAllWithImageDTO>> GetAllShopeWhereSearch(string marketCode, string searchItem);
        public Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAll(string marketCode, string search);
        public Task<List<ShopeControllGetAllWithImageDTO>> SearchShopeAllFromMarket(string search);

    }
}
