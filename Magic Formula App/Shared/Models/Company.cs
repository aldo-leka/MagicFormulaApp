using Microsoft.EntityFrameworkCore;

namespace Shared.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string CIK { get; set; }

        public string CompanyName { get; set; }

        [Precision(19, 4)]
        public decimal LastMarketCapitalization { get; set; }

        [Precision(19, 4)]
        public decimal LastTotalDebt { get; set; }

        [Precision(19, 4)]
        public decimal LastCashAndCashEquivalents { get; set; }

        [Precision(19, 4)]
        public decimal LastNetCurrentAssets { get; set; }

        [Precision(19, 4)]
        public decimal LastNetPropertyPlantAndEquipment { get; set; }

        [Precision(19, 4)]
        public decimal LastOperatingIncome { get; set; }

        [Precision(19, 4)]
        public decimal LastEnterpriseValue { get; set; }

        [Precision(19, 4)]
        public decimal LastEmployedCapital { get; set; }

        public float LastOperatingIncomeToEnterpriseValue { get; set; }

        public float LastReturnOnAssets { get; set; }

        public DateTime LastFilingDate { get; set; }

        //public List<CommonSharesOutstanding> CommonSharesOutstanding { get; set; }
        //public List<AssetsCurrent> AssetsCurrent { get; set; }
        //public List<PropertyPlantAndEquipment> PropertyPlantAndEquipment { get; set; }
        //public List<LiabilitiesCurrent> LiabilitiesCurrent { get; set; }
        //public List<Liabilities> Liabilities { get; set; }
        //public List<OperatingIncome> OperatingIncome { get; set; }
        //public List<CashAndCashEquivalents> CashAndCashEquivalents { get; set; }
        //public List<MarketCapitalization> MarketCapitalization { get; set; }
        //public List<LiabilitiesAndStockholdersEquity> LiabilitiesAndStockholdersEquity { get; set; }
        //public List<Assets> Assets { get; set; }
        //public List<Debt> Debt { get; set; }
    }
}
