using Microsoft.EntityFrameworkCore;

namespace Shared.Models
{
    public class CompanyData : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        //public DbSet<CommonSharesOutstanding> CommonSharesOutstanding { get; set; }
        //public DbSet<AssetsCurrent> AssetsCurrent { get; set; }
        //public DbSet<PropertyPlantAndEquipment> PropertyPlantAndEquipment { get; set; }
        //public DbSet<LiabilitiesCurrent> LiabilitiesCurrent { get; set; }
        //public DbSet<Liabilities> Liabilities { get; set; }
        //public DbSet<OperatingIncome> OperatingIncome { get; set; }
        //public DbSet<CashAndCashEquivalents> CashAndCashEquivalents { get; set; }
        //public DbSet<MarketCapitalization> MarketCapitalization { get; set; }
        //public DbSet<LiabilitiesAndStockholdersEquity> LiabilitiesAndStockholdersEquity { get; set; }
        //public DbSet<Assets> Assets { get; set; }
        //public DbSet<Debt> Debt { get; set; }
    }
}
