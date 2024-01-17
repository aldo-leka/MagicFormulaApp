using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Models
{
    public class SqliteCompanyData : CompanyData
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            options.UseSqlite(configuration.GetConnectionString(DatabaseProvider.Sqlite));
        }
    }
}
