using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFL.Data
{
    public class MarketDbContextFactory
    {
        public static DbContextOptions<MarketDbContext> CreateDbContextOptions(string connectionString)
        {
            // إعدادات DbContextOptions لإنشاء اتصال ديناميكي بناءً على سلسلة الاتصال
            var optionsBuilder = new DbContextOptionsBuilder<MarketDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
    }

}
