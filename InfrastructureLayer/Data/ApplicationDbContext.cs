using DL.Entities;
using Microsoft.EntityFrameworkCore;

namespace INFL.Data
{
    public class ApplicationDbContext : DbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }



        public DbSet<Market> Markets { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Basket_s> Basket_ss { get; set; }

        public DbSet<Dictionary> dictionaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إضافة قيد فريد إلى الحقل ProductName
            modelBuilder.Entity<Dictionary>()
                .HasIndex(d => d.ProductName)
                .IsUnique();
        }

    }
}