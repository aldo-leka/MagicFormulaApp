using Microsoft.EntityFrameworkCore;

namespace SecImporter.Models
{
    public class CompanyData : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<CommonSharesOutstanding> CommonSharesOutstanding { get; set; }
        public string DbPath { get; }

        public CompanyData()
        {
            DbPath = "stocks.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
