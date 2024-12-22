using APPL.DTOs.Response.ShopControle;
using DL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFL.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options) { }
        public DbSet<MarketControl> MarketControls { get; set; }
        public DbSet<ShopControle_LastCode_ResponseDTO> LastCodeQuery { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopControle_LastCode_ResponseDTO>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
