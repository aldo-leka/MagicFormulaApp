using System.ComponentModel.DataAnnotations;

namespace Magic_Formula_App.Models
{
    public class ScreenItem
    {
        [Display(Name = "No.")]
        public int Id { get; set; }

        public string CIK { get; set; }

        public string Ticker { get; set; }

        public string Exchange { get; set; }

        [Display(Name = "Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Market Capitalization (M)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MarketCapitalization { get; set; }

        [Display(Name = "Enterprise Value (M)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal EnterpriseValue { get; set; }

        [Display(Name = "Employed Capital (M)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal EmployedCapital { get; set; }

        [Display(Name = "Operating Income (M)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OperatingIncome { get; set; }

        [Display(Name = "Op. Income / Enterprise Value (%)")]
        public float OperatingIncomeToEnterpriseValue { get; set; }

        [Display(Name = "Return on Employed Capital (%)")]
        public float ReturnOnEmployedCapital { get; set; }

        [Display(Name = "Market Capitalization / Tangible Book (%)")]
        public float PriceToTangibleBook { get; set; }

        [Display(Name = "Market Capitalization / Net Asset Value (%)")]
        public float PriceToNetAssetsValue { get; set; }

        [Display(Name = "Filed At")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FilingDate { get; set; }
    }
}
