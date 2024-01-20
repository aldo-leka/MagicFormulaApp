using Microsoft.EntityFrameworkCore;

namespace Shared.Models
{
    [Keyless]
    public class Fmp
    {
        public string ApiKey { get; set; }
        public int MinimumTimeinSecondsToUpdateMarketCapitalizations { get; set; }
        public int LastBatch { get; set; }
        public DateTime? LastDay { get; set; }
    }
}
