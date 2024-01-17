using Magic_Formula_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Shared;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Magic_Formula_App.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet = true)]
        public decimal MinimumMarketCapitalization { get; set; } = 25;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumOperatingIncomeToEnterpriseValue { get; set; } = 20;

        [BindProperty(SupportsGet = true)]
        [Range(0, 100)]
        public int MinimumReturnOnAssets { get; set; } = 20;

        public IList<ScreenItem> ScreenItems { get; set; } = new List<ScreenItem>();

        private readonly CompanyData _companyData;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            Settings settings = configuration.GetRequiredSection("Settings").Get<Settings>();
            _companyData = settings.DatabaseProvider switch
            {
                DatabaseProvider.SqlServer => new SqlServerCompanyData(),
                DatabaseProvider.Sqlite => new SqliteCompanyData(),
                DatabaseProvider.Postgres => new PostgresCompanyData(),
                _ => throw new Exception($"Unsupported database provider: {settings.DatabaseProvider}"),
            };
        }

        public async Task OnGetAsync()
        {
            // https://www.sec.gov/files/company_tickers_exchange.json
            var httpClient = _httpClientFactory.CreateClient("SecCompanyTickers");

            var httpResponseMessage = await httpClient.GetAsync("files/company_tickers_exchange.json");
            var tickerData = default(TickerData);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
                tickerData = JsonSerializer.Deserialize<TickerData>(jsonString);
            }

            ScreenItems = _companyData.Companies
                .Select(c => new ScreenItem
                {
                    CIK = c.CIK,
                    CompanyName = c.CompanyName,
                    MarketCapitalization = Math.Round(c.LastMarketCapitalization / 1_000_000, 2),
                    EnterpriseValue = Math.Round(c.LastEnterpriseValue / 1_000_000, 2),
                    OperatingIncome = Math.Round(c.LastOperatingIncome / 1_000_000, 2),
                    OperatingIncomeToEnterpriseValue = c.LastOperatingIncomeToEnterpriseValue * 100,
                    NetCurrentAssets = Math.Round(c.LastNetCurrentAssets / 1_000_000, 2),
                    NetPropertyPlantAndEquipment = Math.Round(c.LastNetPropertyPlantAndEquipment / 1_000_000, 2),
                    EmployedCapital = Math.Round(c.LastEmployedCapital / 1_000_000, 2),
                    ReturnOnAssets = c.LastReturnOnAssets * 100,
                    FilingDate = c.LastFilingDate
                })
                .Where(c => c.MarketCapitalization >= MinimumMarketCapitalization
                    && c.OperatingIncomeToEnterpriseValue >= MinimumOperatingIncomeToEnterpriseValue
                    && c.ReturnOnAssets >= MinimumReturnOnAssets)
                .ToList();

            var id = 1;
            foreach (var screenItem in ScreenItems)
            {
                var ticker = tickerData.data.FirstOrDefault(y => y[0].ToString() == screenItem.CIK);
                screenItem.Id = id++;
                screenItem.Ticker = ticker != null ? ticker[2].ToString() : "";
                screenItem.CompanyName = ticker != null ? ticker[1].ToString() : screenItem.CompanyName;
            }

            await Task.CompletedTask;
        }
    }
}
