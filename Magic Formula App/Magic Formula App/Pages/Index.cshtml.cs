using Magic_Formula_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Magic_Formula_App.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, CompanyData context) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;

        [BindProperty(SupportsGet = true)]
        public decimal MinimumMarketCapitalization { get; set; } = 20;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumOperatingIncomeToEnterpriseValue { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumReturnOnAssets { get; set; } = 0;

        public IList<ScreenItem> ScreenItems { get; set; } = new List<ScreenItem>();

        private readonly CompanyData _context = context;

        public async Task OnGetAsync()
        {
            ScreenItems = _context.Companies
                .Select(c => new ScreenItem
                {
                    CIK = c.CIK,
                    Ticker = c.Ticker,
                    Exchange = c.Exchange,
                    CompanyName = c.CompanyName,
                    MarketCapitalization = Math.Round(c.LastMarketCapitalization / 1_000_000, 2),
                    EnterpriseValue = Math.Round(c.EnterpriseValue / 1_000_000, 2),
                    EmployedCapital = Math.Round(c.EmployedCapital / 1_000_000, 2),
                    OperatingIncome = Math.Round(c.OperatingIncome / 1_000_000, 2),
                    OperatingIncomeToEnterpriseValue = c.OperatingIncomeToEnterpriseValue * 100,
                    ReturnOnAssets = c.ReturnOnEmployedCapital * 100,
                    FilingDate = c.LastFilingDate
                })
                .Where(c => c.MarketCapitalization >= MinimumMarketCapitalization
                    && c.OperatingIncomeToEnterpriseValue >= MinimumOperatingIncomeToEnterpriseValue
                    && c.ReturnOnAssets >= MinimumReturnOnAssets)
                .ToList();

            var id = 1;
            foreach (var item in ScreenItems)
            {
                item.Id = id++;
            }

            await Task.CompletedTask;
        }
    }
}
