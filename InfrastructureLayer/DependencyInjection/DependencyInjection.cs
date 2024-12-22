

using APPL.Contract.Basket_ss;
using APPL.Contract.Baskets;
using APPL.Contract.Cities;
using APPL.Contract.Customers;
using APPL.Contract.Dictionarys;
using APPL.Contract.Market;
using APPL.Contract.MarketControl;
using APPL.Contract.Shops;
using APPL.Contract.Vendor;
using Dapper;
using INFL.Data;
using INFL.Repositories.Basket_ss;
using INFL.Repositories.Baskets;
using INFL.Repositories.CitiesR;
using INFL.Repositories.Customers;
using INFL.Repositories.Dictionarys;
using INFL.Repositories.MarketControl;
using INFL.Repositories.Markets;
using INFL.Repositories.Shpos;
using INFL.Repositories.VendorRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace INFL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString, IConfiguration configuration)
        {
            // إعداد DbContext واستخدام SQL Server كقاعدة البيانات
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)); // تحديد SQL Server كقاعدة البيانات
            services.AddScoped<MarketDbContextFactory>();
            // إضافة المستودعات العامة والخاصة
            services.Configure<PathSettings>(configuration.GetSection("MPaths"));
            services.AddScoped<IMarketControl, MarketControlRepo>();
            services.AddScoped<ICity, CityRepo>();

            services.AddScoped<IMarekrt, MarketRepo>();
            services.AddScoped<IVendors, VendorRepo>();
            services.AddScoped<IShops, ShopRepo>();
            services.AddScoped<ICustomer, CustomerRepo>();
            services.AddScoped<IBasketRepo, BasketRepo>();
            services.AddScoped<IBasket_sRepo, Basket_sRepo>();
            services.AddScoped<IDictionaryRepository, DictionaryRepository>();


            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
            });

            SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

            return services;
        }
    }
}
