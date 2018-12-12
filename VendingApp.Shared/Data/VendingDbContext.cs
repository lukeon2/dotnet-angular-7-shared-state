using Microsoft.EntityFrameworkCore;
using VendingApp.Infrastructure.Entities;

namespace VendingApp.Infrastructure.Data
{
    public class VendingDbContext : DbContext
    {
        public VendingDbContext(DbContextOptions options) : base(options){ }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        @"Server=(localdb)\MSSQLLocalDB;Database=local_db;Trusted_Connection=True;MultipleActiveResultSets=true");
        //}

        public DbSet<Config> Config { get; set; }

        public DbSet<ExchangeRate> ExchangeRate { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<Currency> Currency { get; set; }
    }
}