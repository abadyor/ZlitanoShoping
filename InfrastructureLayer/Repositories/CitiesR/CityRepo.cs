using APPL.Contract.Cities;
using APPL.DTOs.Request.Cities;
using APPL.DTOs.Response.Cities;
using APPL.DTOS.Response;
using DL.Entities;
using INFL.Data;
using Microsoft.EntityFrameworkCore;


namespace INFL.Repositories.CitiesR
{
    public class CityRepo : ICity
    {
        private readonly ApplicationDbContext _context;

        public CityRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        // جلب المدينة بواسطة المعرف
        // جلب المدينة بواسطة المعرف
        public async Task<CitiesResponseDTO> GetByIdAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return null;  // إرجاع null إذا لم يتم العثور على المدينة
            }

            // تحويل كائن Cities إلى CitiesResponseDTO
            var cityDto = new CitiesResponseDTO
            {
                Id = city.Id,
                CCode = city.CCode,
                Name = city.Name
            };

            return cityDto;
        }

        // جلب جميع المدن
        public async Task<IEnumerable<CitiesResponseDTO>> GetAllAsync()
        {
            var cities = await _context.Cities.ToListAsync();

            var citiesDto = cities.Select(city => new CitiesResponseDTO
            {
                Id = city.Id,
                CCode = city.CCode,
                Name = city.Name
            });

            return citiesDto;
        }

        // إنشاء مدينة جديدة
        public async Task<GeneralRespond> CreateAsync(CreateCitiesRequestDTO cityCreateDto)
        {
            // Retrieve the last city code in the database
            var lastCity = await _context.Cities
                                  .OrderByDescending(c => c.CCode)
                                  .FirstOrDefaultAsync();

            // Generate the next city code based on the last code
            var nextCode = lastCity != null ? CodeGenerator.GenerateNextCode(lastCity.CCode) : "01";

            var city = new Cities
            {
                CCode = nextCode,
                Name = cityCreateDto.Name
            };

            try
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();

                return new GeneralRespond
                {
                    Flag = true,
                    Message = "City created successfully.",
                };
            }
            catch (Exception ex)
            {
                return new GeneralRespond
                {
                    Flag = false,
                    Message = $"Error creating city: {ex.Message}"
                };
            }
        }


        // تحديث بيانات مدينة
        public async Task<GeneralRespond> UpdateAsync(UpdateCitiesRequestDTO cityUpdateDto)
        {
            var city = await _context.Cities.FindAsync(cityUpdateDto.Id);
            if (city == null)
            {
                return new GeneralRespond
                {
                    Flag = false,
                    Message = "City not found."
                };
            }

            city.CCode = cityUpdateDto.CCode;
            city.Name = cityUpdateDto.Name;

            try
            {
                _context.Cities.Update(city);
                await _context.SaveChangesAsync();

                return new GeneralRespond
                {
                    Flag = true,
                    Message = "City updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralRespond
                {
                    Flag = false,
                    Message = $"Error updating city: {ex.Message}"
                };
            }
        }

        // حذف مدينة
        public async Task<GeneralRespond> DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return new GeneralRespond
                {
                    Flag = false,
                    Message = "City not found."
                };
            }

            try
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();

                return new GeneralRespond
                {
                    Flag = true,
                    Message = "City deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralRespond
                {
                    Flag = false,
                    Message = $"Error deleting city: {ex.Message}"
                };
            }
        }
    }
}