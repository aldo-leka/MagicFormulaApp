using Magic_Formula_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Models;

namespace Magic_Formula_App.Pages
{
    public class LowPriceToBookStocksModel(ILogger<IndexModel> logger, CompanyData context, IConfiguration configuration) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly CompanyData _context = context;
        private readonly IConfiguration _configuration = configuration;

        [BindProperty(SupportsGet = true)]
        public decimal MinimumMarketCapitalization { get; set; } = 3;

        public PaginatedList<ScreenItem> ScreenItems { get; set; }

        public string MarketCapitalizationSort { get; set; }
        public string ReturnOnEmployedCapitalSort { get; set; }
        public string PriceToTangibleBookSort { get; set; }
        public string PriceToNetAssetValueSort { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder, int? pageIndex)
        {
            CurrentSort = sortOrder;
            MarketCapitalizationSort = string.IsNullOrEmpty(sortOrder) ? "MarketCapitalization_desc" : "";
            ReturnOnEmployedCapitalSort = sortOrder == "ReturnOnEmployedCapital" ? "ReturnOnEmployedCapital_desc" : "ReturnOnEmployedCapital";
            PriceToTangibleBookSort = sortOrder == "PriceToTangibleBook" ? "PriceToTangibleBook_desc" : "PriceToTangibleBook";
            PriceToNetAssetValueSort = sortOrder == "PriceToNetAssetValueSort" ? "PriceToNetAssetValueSort_desc" : "PriceToNetAssetValueSort";

            IQueryable <ScreenItem> screenItems = _context.Companies
                .Select(c => new
                {
                    c.CIK,
                    c.Ticker,
                    c.Exchange,
                    c.CompanyName,
                    CurrentAssets = Math.Round(c.CurrentAssets / 1_000_000, 2),
                    PropertyPlantAndEquipment = Math.Round(c.PropertyPlantAndEquipment / 1_000_000, 2),
                    Assets = Math.Round(c.Assets / 1_000_000, 2),
                    IntangibleAssets = Math.Round(c.IntangibleAssets / 1_000_000, 2),
                    MarketCapitalization = Math.Round(c.LastMarketCapitalization / 1_000_000, 2),
                    Liabilities = Math.Round(c.Liabilities / 1_000_000, 2),
                    OperatingIncome = Math.Round(c.OperatingIncome / 1_000_000, 2),
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
                    c.FilingDate,
                    EmployedCapital = c.CurrentAssets + c.PropertyPlantAndEquipment,
                    TangibleBookValue = c.Assets - c.IntangibleAssets - c.Liabilities,
                    NetAssetValue = c.CurrentAssets - c.Liabilities
                })
                .Where(c => c.TangibleBookValue > 0)
                .Select(c => new ScreenItem
                {
                    CIK = c.CIK,
                    Ticker = c.Ticker,
                    Exchange = c.Exchange,
                    CompanyName = c.CompanyName,
                    MarketCapitalization = c.MarketCapitalization,
                    OperatingIncome = c.OperatingIncome,
                    FilingDate = c.FilingDate,
                    ReturnOnEmployedCapital = (float)(c.EmployedCapital != 0 ? c.OperatingIncome / c.EmployedCapital : 0) * 100,
                    PriceToTangibleBook = (float)(c.TangibleBookValue != 0 ? c.MarketCapitalization / c.TangibleBookValue : 0) * 100,
                    PriceToNetAssetsValue = (float)(c.NetAssetValue != 0 ? c.MarketCapitalization / c.NetAssetValue : 0) * 100
                })
                .Where(c => c.MarketCapitalization >= MinimumMarketCapitalization && c.PriceToTangibleBook < 100);

            screenItems = sortOrder switch
            {
                "MarketCapitalization_desc" => screenItems.OrderByDescending(c => c.MarketCapitalization),
                "ReturnOnEmployedCapital" => screenItems.OrderBy(c => c.ReturnOnEmployedCapital),
                "ReturnOnEmployedCapital_desc" => screenItems.OrderByDescending(c => c.ReturnOnEmployedCapital),
                "PriceToTangibleBook" => screenItems.OrderBy(c => c.PriceToTangibleBook),
                "PriceToTangibleBook_desc" => screenItems.OrderByDescending(c => c.PriceToTangibleBook),
                "PriceToNetAssetValueSort" => screenItems.OrderBy(c => c.PriceToNetAssetsValue),
                "PriceToNetAssetValueSort_desc" => screenItems.OrderByDescending(c => c.PriceToNetAssetsValue),
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
