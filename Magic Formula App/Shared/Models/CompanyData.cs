using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Models
{
    public class CompanyData : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Fmp> Fmp { get; set; }

        private readonly bool _initialized;

        public CompanyData()
        {
            _initialized = false;
        }

        public CompanyData(DbContextOptions<CompanyData> options) : base(options)
        {
            _initialized = true;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!_initialized)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }
    }
}
