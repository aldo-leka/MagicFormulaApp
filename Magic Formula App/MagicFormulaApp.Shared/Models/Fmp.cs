namespace MagicFormulaApp.Shared.Models
{
    public class Fmp
    {
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public int MinimumTimeinSecondsToUpdateMarketCapitalizations { get; set; }
        public int MaxRequestsPerDay { get; set; }
        public int LastBatch { get; set; }
        public DateTime? LastDay { get; set; }
    }
}
