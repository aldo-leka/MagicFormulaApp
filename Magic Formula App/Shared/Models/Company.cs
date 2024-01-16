namespace SecImporter.Models
{
    public class Company
    {
        // Known data
        public int Id { get; set; }
        public string CIK { get; set; }
        public string CompanyName { get; set; }

        public List<CommonSharesOutstanding> CommonSharesOutstanding { get; set; }
    }
}
