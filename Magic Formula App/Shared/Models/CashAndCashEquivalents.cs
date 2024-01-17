namespace Shared.Models
{
    public class CashAndCashEquivalents
    {
        public int Id { get; set; }
        public DateTime End { get; set; }
        public double Value { get; set; }
        public int? FiscalYear { get; set; }
        public string FiscalPart { get; set; } = "";
        public string Form { get; set; } = "";
        public DateTime Filed { get; set; }

        public Company Company { get; set; }
    }
}
