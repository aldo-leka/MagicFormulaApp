namespace Updater
{
    public class CompanyField
    {
        public Dictionary<string, DateTime?> KeyDate { get; set; }

        public Dictionary<string, DateTime?> BackupKeyDate { get; set; }

        public DateTime? LastFilingDate { get; set; }

        public decimal? Value { get; set; }
    }
}
