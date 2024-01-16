using Magic_Formula_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Magic_Formula_App.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet = true)]
        public int? MarketCapitalization { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ReturnOnAssets { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? OperatingIncomeToEnterpriseValue { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Sector { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Country { get; set; }

        public IList<ScreenItem> ScreenItems { get; set; } = default!;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var log = (MarketCapitalization.HasValue ? MarketCapitalization.ToString() : "\nNo market cap")
                + $"\n{(ReturnOnAssets.HasValue ? ReturnOnAssets.ToString() : "No ROA")}"
                + $"\n{(OperatingIncomeToEnterpriseValue.HasValue ? OperatingIncomeToEnterpriseValue.ToString() : "No Op Inc/EV")}"
                + $"\n{(!string.IsNullOrEmpty(Sector) ? Sector : "No Sector")}"
                + $"\n{(!string.IsNullOrEmpty(Country) ? Sector : "No Country")}";

            _logger.LogInformation(log);

            await Task.CompletedTask;
        }
    }
}
