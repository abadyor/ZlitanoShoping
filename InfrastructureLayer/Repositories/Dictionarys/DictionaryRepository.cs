using APPL.Contract.Dictionarys;
using APPL.DTOs.Request.Dictionarys;
using APPL.DTOs.Response.Dictionarys;
using DL.Entities;
using INFL.Data;
using Microsoft.EntityFrameworkCore;

namespace INFL.Repositories.Dictionarys
{
    public class DictionaryRepository : IDictionaryRepository
    {
        private readonly ApplicationDbContext _context;

        public DictionaryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // إحضار جميع البيانات
        public async Task<IEnumerable<DictionaryDTO>> GetAllAsync()
        {
            return await _context.dictionaries
                                 .Select(d => new DictionaryDTO
                                 {
                                     Id = d.Id,
                                     ProductName = d.ProductName,
                                     Description = d.Description,
                                     TableName = d.TableName
                                 })
                                 .ToListAsync();
        }

        // إحضار عنصر حسب المعرف
        public async Task<DictionaryDTO> GetByIdAsync(int id)
        {
            var dictionary = await _context.dictionaries
                                           .Where(d => d.Id == id)
                                           .FirstOrDefaultAsync();

            if (dictionary == null)
            {
                return null;
            }

            return new DictionaryDTO
            {
                Id = dictionary.Id,
                ProductName = dictionary.ProductName,
                Description = dictionary.Description,
                TableName = dictionary.TableName
            };
        }

        // إضافة عنصر
        public async Task AddAsync(AddDictionaryDTO addDto)
        {
            var dictionary = new Dictionary
            {
                ProductName = addDto.ProductName,
                Description = addDto.Description,
                TableName = addDto.TableName
            };

            await _context.dictionaries.AddAsync(dictionary);
            await _context.SaveChangesAsync();
        }

        // تعديل عنصر
        public async Task UpdateAsync(UpdateDictionaryDTO updateDto)
        {
            var dictionary = await _context.dictionaries
                                           .Where(d => d.Id == updateDto.Id)
                                           .FirstOrDefaultAsync();

            if (dictionary != null)
            {
                dictionary.ProductName = updateDto.ProductName;
                dictionary.Description = updateDto.Description;
                dictionary.TableName = updateDto.TableName;

                _context.dictionaries.Update(dictionary);
                await _context.SaveChangesAsync();
            }
        }

        // حذف عنصر
        public async Task DeleteAsync(DeleteDictionaryDTO deleteDto)
        {
            var dictionary = await _context.dictionaries
                                           .Where(d => d.Id == deleteDto.Id)
                                           .FirstOrDefaultAsync();

            if (dictionary != null)
            {
                _context.dictionaries.Remove(dictionary);
                await _context.SaveChangesAsync();
            }
        }


        public async Task AddOrUpdateProductAsync(string productName, string description, string tableName)
        {
            // البحث عن المنتج باستخدام اسم المنتج
            var dictionary = await _context.dictionaries
                                           .Where(d => d.ProductName == productName)
                                           .FirstOrDefaultAsync();

            if (dictionary == null) // إذا كان المنتج غير موجود
            {
                // إضافة منتج جديد مع اسم الجدول
                var newDictionary = new Dictionary
                {
                    ProductName = productName,
                    Description = description,
                    TableName = tableName // تخزين اسم الجدول كسلسلة نصية
                };

                await _context.dictionaries.AddAsync(newDictionary);
            }
            else
            {
                // إذا كان المنتج موجودًا، تحقق من وجود اسم الجدول
                if (!dictionary.TableName.Contains(tableName))
                {
                    // إذا لم يكن اسم الجدول موجودًا، أضفه مع فاصلة
                    if (!string.IsNullOrEmpty(dictionary.TableName))
                    {
                        dictionary.TableName += $",{tableName}"; // إضافة الفاصلة قبل اسم الجدول
                    }
                    else
                    {
                        dictionary.TableName = tableName; // إذا كانت السلسلة فارغة، نضيف اسم الجدول مباشرة
                    }

                    // تحديث العنصر في قاعدة البيانات
                    _context.dictionaries.Update(dictionary);
                }
            }

            // حفظ التغييرات في قاعدة البيانات
            await _context.SaveChangesAsync();
        }


        public async Task<List<DictionaryDTO>> SearchProductsAsync(string searchTerm)
        {
            // استعلام البيانات من DB
            var dictionaries = await _context.dictionaries
                                             .Where(d => d.ProductName.Contains(searchTerm) || d.Description.Contains(searchTerm))
                                             .ToListAsync();

            // تحويل البيانات من DL.Entities.Dictionary إلى DictionaryDTO
            return dictionaries.Select(d => new DictionaryDTO
            {
                Id = d.Id,
                ProductName = d.ProductName,
                Description = d.Description,
                TableName = d.TableName
            }).ToList();
        }



    }
}
