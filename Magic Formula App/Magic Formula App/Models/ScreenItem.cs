namespace Magic_Formula_App.Models
{
    public class ScreenItem
    {
        public string? Ticker { get; set; }
        public string? CompanyName { get; set; }
        public int? MarketCapitalization { get; set; }
        public int? EnterpriseValue { get; set; }
        public int? OperatingIncome { get; set; }
        public float? OperatingIncomeToEnterpriseValue { get; set; }
        public int? NetCurrentAssets { get; set; }
        public int? NetPropertyPlantAndEquipment { get; set; }
        public int? EmployedCapital { get; set; }
        public float? ReturnOnAssets { get; set; }
        public string? Sector { get; set; }
        public string? Country { get; set; }
    }
}
