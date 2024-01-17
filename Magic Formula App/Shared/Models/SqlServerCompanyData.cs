using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Models
{
    public class SqlServerCompanyData() : CompanyData
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            options.UseSqlServer(configuration.GetConnectionString(DatabaseProvider.SqlServer));
        }
    }
}
