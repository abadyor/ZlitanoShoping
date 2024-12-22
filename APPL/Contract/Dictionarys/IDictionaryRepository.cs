using APPL.DTOs.Request.Dictionarys;
using APPL.DTOs.Response.Dictionarys;

namespace APPL.Contract.Dictionarys
{
    public interface IDictionaryRepository
    {
        Task<IEnumerable<DictionaryDTO>> GetAllAsync();             // إحضار جميع البيانات
        Task<DictionaryDTO> GetByIdAsync(int id);                   // إحضار عنصر حسب المعرف
        Task AddAsync(AddDictionaryDTO addDto);                     // إضافة عنصر
        Task UpdateAsync(UpdateDictionaryDTO updateDto);            // تعديل عنصر
        Task DeleteAsync(DeleteDictionaryDTO deleteDto);

        Task AddOrUpdateProductAsync(string productName, string Discription, string tableName);
        Task<List<DictionaryDTO>> SearchProductsAsync(string searchTerm);
    }
}
