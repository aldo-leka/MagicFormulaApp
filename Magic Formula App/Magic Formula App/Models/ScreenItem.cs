using System.ComponentModel.DataAnnotations;

namespace Magic_Formula_App.Models
{
    public class ScreenItem
    {
        [Display(Name = "No.")]
        public int Id { get; set; }

        public string CIK { get; set; }

        public string Ticker { get; set; }

        [Display(Name = "Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Market Capitalization (M)")]
        [DataType(DataType.Currency)]
        public decimal MarketCapitalization { get; set; }

        [Display(Name = "Enterprise Value (M)")]
        [DataType(DataType.Currency)]
        public decimal EnterpriseValue { get; set; }

        [Display(Name = "Operating Income (M)")]
        [DataType(DataType.Currency)]
        public decimal OperatingIncome { get; set; }

        [Display(Name = "Op. Income / Enterprise Value (%)")]
        public float OperatingIncomeToEnterpriseValue { get; set; }

        [Display(Name = "Current Assets (M)")]
        [DataType(DataType.Currency)]
        public decimal NetCurrentAssets { get; set; }

        [Display(Name = "PPE (M)")]
        [DataType(DataType.Currency)]
        public decimal NetPropertyPlantAndEquipment { get; set; }

        [Display(Name = "Employed Capital (M)")]
        [DataType(DataType.Currency)]
        public decimal EmployedCapital { get; set; }

        [Display(Name = "ROA (%)")]
        public float ReturnOnAssets { get; set; }

        [Display(Name = "Filed")]
        [DataType(DataType.Date)]
        public DateTime FilingDate { get; set; }
    }
}
