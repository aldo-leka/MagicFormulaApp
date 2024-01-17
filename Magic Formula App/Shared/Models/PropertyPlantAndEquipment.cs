namespace Shared.Models
{
    public class PropertyPlantAndEquipment
    {
        public int Id { get; set; }
        public DateTime End { get; set; }
        public decimal Value { get; set; }
        public int? FiscalYear { get; set; }
        public string FiscalPart { get; set; } = "";
        public string Form { get; set; } = "";
        public DateTime Filed { get; set; }

        public Company Company { get; set; }
    }
}
