using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Models
{
    public class PostgresCompanyData : CompanyData
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            options.UseNpgsql(configuration.GetConnectionString(DatabaseProvider.Postgres));
        }
    }
}
