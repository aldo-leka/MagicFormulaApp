using Magic_Formula_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MagicFormulaApp.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Magic_Formula_App.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, CompanyData context, IConfiguration configuration) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly CompanyData _context = context;
        private readonly IConfiguration _configuration = configuration;

        [BindProperty(SupportsGet = true)]
        public decimal MinimumMarketCapitalization { get; set; } = 20;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumOperatingIncomeToEnterpriseValue { get; set; } = 18;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumReturnOnEmployedCapital { get; set; } = 18;

        public PaginatedList<ScreenItem> ScreenItems { get; set; }

        public string MarketCapitalizationSort { get; set; }
        public string OperatingIncomeToEnterpriseValueSort { get; set; }
        public string ReturnOnEmployedCapitalSort { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder, int? pageIndex)
        {
            CurrentSort = sortOrder;
            MarketCapitalizationSort = string.IsNullOrEmpty(sortOrder) ? "MarketCapitalization_desc" : "";
            OperatingIncomeToEnterpriseValueSort = sortOrder == "OperatingIncomeToEnterpriseValue" ? "OperatingIncomeToEnterpriseValue_desc" : "OperatingIncomeToEnterpriseValue";
            ReturnOnEmployedCapitalSort = sortOrder == "ReturnOnEmployedCapital" ? "ReturnOnEmployedCapital_desc" : "ReturnOnEmployedCapital";

            IQueryable<ScreenItem> screenItems = _context.Companies
                .Select(c => new
                {
                    c.CIK,
                    c.Ticker,
                    c.Exchange,
                    c.CompanyName,
                    CashAndCashEquivalents = Math.Round(c.CashAndCashEquivalents / 1_000_000, 2),
                    CurrentAssets = Math.Round(c.CurrentAssets / 1_000_000, 2),
                    PropertyPlantAndEquipment = Math.Round(c.PropertyPlantAndEquipment / 1_000_000, 2),
                    Debt = Math.Round(c.Debt / 1_000_000, 2),
                    MarketCapitalization = Math.Round(c.LastMarketCapitalization / 1_000_000, 2),
                    OperatingIncome = Math.Round(c.OperatingIncome / 1_000_000, 2),
                    Liabilities = Math.Round(c.Liabilities / 1_000_000, 2),
                    FilingDate = c.LastFilingDate
                })
                .Select(c => new
                {
                    c.CIK,
                    c.Ticker,
                    c.Exchange,
                    c.CompanyName,
                    c.MarketCapitalization,
                    c.OperatingIncome,
                    EnterpriseValue = c.MarketCapitalization + c.Debt - c.CashAndCashEquivalents,
                    EmployedCapital = c.CurrentAssets + c.PropertyPlantAndEquipment,
                    c.FilingDate
                })
                .Select(c => new ScreenItem
                {
                    CIK = c.CIK,
                    Ticker = c.Ticker,
                    Exchange = c.Exchange,
                    CompanyName = c.CompanyName,
                    MarketCapitalization = c.MarketCapitalization,
                    OperatingIncome = c.OperatingIncome,
                    EnterpriseValue = c.EnterpriseValue,
                    EmployedCapital = c.EmployedCapital,
                    OperatingIncomeToEnterpriseValue = (float)(c.EnterpriseValue != 0 ? c.OperatingIncome / c.EnterpriseValue : 0) * 100,
                    ReturnOnEmployedCapital = (float)(c.EmployedCapital != 0 ? c.OperatingIncome / c.EmployedCapital : 0) * 100,
                    FilingDate = c.FilingDate
                })
                .Where(c => c.MarketCapitalization >= MinimumMarketCapitalization
                    && c.OperatingIncomeToEnterpriseValue >= MinimumOperatingIncomeToEnterpriseValue
                    && c.ReturnOnEmployedCapital >= MinimumReturnOnEmployedCapital);

            screenItems = sortOrder switch
            {
                "MarketCapitalization_desc" => screenItems.OrderByDescending(c => c.MarketCapitalization),
                "OperatingIncomeToEnterpriseValue" => screenItems.OrderBy(c => c.OperatingIncomeToEnterpriseValue),
                "OperatingIncomeToEnterpriseValue_desc" => screenItems.OrderByDescending(c => c.OperatingIncomeToEnterpriseValue),
                "ReturnOnEmployedCapital" => screenItems.OrderBy(c => c.ReturnOnEmployedCapital),
                "ReturnOnEmployedCapital_desc" => screenItems.OrderByDescending(c => c.ReturnOnEmployedCapital),
                _ => screenItems.OrderBy(c => c.MarketCapitalization),
            };

            var pageSize = _configuration.GetValue("PageSize", 10);
            ScreenItems = await PaginatedList<ScreenItem>.CreateAsync(screenItems, pageIndex ?? 1, pageSize);

            var id = ((pageIndex - 1) ?? 0) * pageSize + 1;
            foreach (var item in ScreenItems)
            {
                item.Id = id++;
            }
        }
    }
}
