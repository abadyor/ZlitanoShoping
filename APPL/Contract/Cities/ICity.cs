using APPL.DTOs.Request.Cities;
using APPL.DTOs.Response.Cities;
using APPL.DTOS.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.Contract.Cities
{
    public interface ICity
    {
        Task<CitiesResponseDTO> GetByIdAsync(int id);  // جلب المدينة بواسطة المعرف
        Task<IEnumerable<CitiesResponseDTO>> GetAllAsync();  // جلب جميع المدن
        Task<GeneralRespond> CreateAsync(CreateCitiesRequestDTO cityCreateDto);  // إنشاء مدينة جديدة
        Task<GeneralRespond> UpdateAsync(UpdateCitiesRequestDTO cityUpdateDto);  // تحديث بيانات مدينة
        Task<GeneralRespond> DeleteAsync(int id);  // حذف مدينة
    }
}
