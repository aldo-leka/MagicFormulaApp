using Magic_Formula_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Models;
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
        public int MinimumOperatingIncomeToEnterpriseValue { get; set; } = 15;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumReturnOnEmployedCapital { get; set; } = 15;

        public PaginatedList<ScreenItem> ScreenItems { get; set; }

        public string MarketCapitalizationSort { get; set; }
        public string OperatingIncomeToEnterpriseValueSort { get; set; }
        public string ReturnOnAssetsSort { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder, int? pageIndex)
        {
            CurrentSort = sortOrder;
            MarketCapitalizationSort = string.IsNullOrEmpty(sortOrder) ? "MarketCapitalization_desc" : "";
            OperatingIncomeToEnterpriseValueSort = sortOrder == "OperatingIncomeToEnterpriseValue" ? "OperatingIncomeToEnterpriseValue_desc" : "OperatingIncomeToEnterpriseValue";
            ReturnOnAssetsSort = sortOrder == "ReturnOnAssets" ? "ReturnOnAssets_desc" : "ReturnOnAssets";

            IQueryable<Company> companies = _context.Companies
                .Where(c => c.LastMarketCapitalization >= MinimumMarketCapitalization * 1_000_000
                    && c.OperatingIncomeToEnterpriseValue >= MinimumOperatingIncomeToEnterpriseValue / 100f
                    && c.ReturnOnEmployedCapital >= MinimumReturnOnEmployedCapital / 100f);

            switch (sortOrder)
            {
                case "MarketCapitalization_desc":
                    companies = companies.OrderByDescending(c => c.LastMarketCapitalization);
                    break;
                case "OperatingIncomeToEnterpriseValue":
                    companies = companies.OrderBy(c => c.OperatingIncomeToEnterpriseValue);
                    break;
                case "OperatingIncomeToEnterpriseValue_desc":
                    companies = companies.OrderByDescending(c => c.OperatingIncomeToEnterpriseValue);
                    break;
                case "ReturnOnAssets":
                    companies = companies.OrderBy(c => c.ReturnOnEmployedCapital);
                    break;
                case "ReturnOnAssets_desc":
                    companies = companies.OrderByDescending(c => c.ReturnOnEmployedCapital);
                    break;
                default:
                    companies = companies.OrderBy(c => c.LastMarketCapitalization);
                    break;
            }

            var screenItems = companies.Select(c => new ScreenItem
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
            });

            var pageSize = _configuration.GetValue("PageSize", 15);
            ScreenItems = await PaginatedList<ScreenItem>.CreateAsync(screenItems, pageIndex ?? 1, pageSize);

            var id = ((pageIndex - 1) ?? 0) * pageSize + 1;
            foreach (var item in ScreenItems)
            {
                item.Id = id++;
            }
        }
    }
}
