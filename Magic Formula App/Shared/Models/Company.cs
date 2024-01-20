using Microsoft.EntityFrameworkCore;

namespace Shared.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string CIK { get; set; }

        public string Ticker { get; set; }

        public string Exchange { get; set; }

        public string CompanyName { get; set; }

        [Precision(19, 4)]
        public decimal LastMarketCapitalization { get; set; }

        [Precision(19, 4)]
        public decimal CashAndCashEquivalents { get; set; }

        [Precision(19, 4)]
        public decimal CurrentAssets { get; set; }

        [Precision(19, 4)]
        public decimal PropertyPlantAndEquipment { get; set; }

        [Precision(19, 4)]
        public decimal Assets { get; set; }

        [Precision(19, 4)]
        public decimal TotalDebt { get; set; }

        [Precision(19, 4)]
        public decimal Liabilities { get; set; }

        [Precision(19, 4)]
        public decimal OperatingIncome { get; set; }

        [Precision(19, 4)]
        public decimal NetCurrentAssets { get; set; }

        [Precision(19, 4)]
        public decimal TangibleAssets { get; set; }

        [Precision(19, 4)]
        public decimal EmployedCapital { get; set; }

        [Precision(19, 4)]
        public decimal EnterpriseValue { get; set; }

        public float ReturnOnEmployedCapital { get; set; }

        public float OperatingIncomeToEnterpriseValue { get; set; }

        public DateTime LastFilingDate { get; set; }

        public DateTime? LastMarketCapitalizationDate { get; set; }
    }
}
