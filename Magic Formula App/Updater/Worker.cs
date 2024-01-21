using MatthiWare.FinancialModelingPrep;
using MatthiWare.FinancialModelingPrep.Model;
using MatthiWare.FinancialModelingPrep.Model.CompanyValuation;
using Shared.Models;
using System.Text.Json;

namespace Updater
{
    public class Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        private int? _currentBatch;
        private int _fmpCalls;
        private TickerData _tickerData;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_tickerData == null)
                {
                    _logger.LogInformation("No ticker data available. Fetching ticker data...");
                    var httpClient = _httpClientFactory.CreateClient("SecClient");

                    var httpResponseMessage = await httpClient.GetAsync(_configuration["TickerDataDownloadUrlLocation"], stoppingToken);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var jsonString = await httpResponseMessage.Content.ReadAsStringAsync(stoppingToken);
                        _tickerData = JsonSerializer.Deserialize<TickerData>(jsonString);
                    }
                    else
                    {
                        _logger.LogInformation("Fetching ticker data failed.\n{responseContent}", httpResponseMessage.Content);
                    }
                }

                var companyFactsFolder = new DirectoryInfo(_configuration["CompanyFactsFolder"]);
                if (companyFactsFolder.Exists)
                {
                    var batchSize = _configuration.GetValue("BatchSize", 100);
                    _currentBatch ??= 1;

                    _logger.LogInformation("Analyzing current batch: {currentBatch} of company files at {companyFactsFolder}.", _currentBatch, companyFactsFolder.FullName);

                    IReadOnlyList<FileInfo> files = companyFactsFolder.GetFiles("*.json");
                    List<FileInfo> filesToRead = files
                        .OrderBy(fi => fi.Name, StringComparer.OrdinalIgnoreCase)
                        .Skip((_currentBatch.Value - 1) * batchSize)
                        .Take(batchSize)
                        .ToList();

                    List<Task<Root>> tasks = filesToRead
                        .Select(fi => ReadJsonFileAsync<Root>(fi.FullName))
                        .ToList();

                    var items = await Task.WhenAll(tasks);

                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<CompanyData>();
                    var fmpConfig = context.Fmp.SingleOrDefault();

                    IFinancialModelingPrepApiClient fmpApiClient = default;
                    if (fmpConfig != null)
                    {
                        fmpApiClient = FinancialModelingPrepApiClientFactory.CreateClient(new FinancialModelingPrepOptions
                        {
                            ApiKey = fmpConfig.ApiKey
                        });
                    }

                    List<Task<ApiResponse<QuoteResponse>>> fmpTasks = [];

                    foreach (var item in items)
                    {
                        if (item.cik != null && item.entityName != null && item.facts.usgaap != null)
                        {
                            var cik = item.cik.ToString();
                            var tickerData = _tickerData.data.FirstOrDefault(y => y[0].ToString() == cik);
                            if (tickerData != null)
                            {
                                // sometimes these fields are null so no filed is available.
                                // they should be considered with value 0 in these situations.
                                // and filed shouldn't matter.
                                // if they're non-null, filed date matters because i want the latest financial data snapshot to have the same filing date.
                                // If the date of any recent financial data is old, it simply means value should be treated as zero as it has had no update.

                                var filedDate = default(DateTime?);
                                var cashAndCashEquivalents = new USD { val = 0 };
                                var currentAssets = new USD { val = 0 };
                                var propertyPlantAndEquipment = new USD { val = 0 };
                                var goodwill = new USD { val = 0 };
                                var intangibleAssets = new USD { val = 0 };
                                var trademarks = new USD { val = 0 }; 
                                var assets = new USD { val = 0 };
                                var longTermDebt = new USD { val = 0 };
                                var currentDebt = new USD { val = 0 };
                                var liabilities = new USD { val = 0 };
                                var operatingIncome = new USD { val = 0 };
                                
                                bool cashAndCashEquivalentsAvailable = item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue != null && item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD != null && item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD.Count > 0;
                                bool cashAndCashEquivalentsBackupAvailable = item.facts.usgaap.Cash != null && item.facts.usgaap.Cash.units.USD != null && item.facts.usgaap.Cash.units.USD.Count > 0;
                                bool currentAssetsAvailable = item.facts.usgaap.AssetsCurrent != null && item.facts.usgaap.AssetsCurrent.units.USD != null && item.facts.usgaap.AssetsCurrent.units.USD.Count > 0;
                                bool propertyPlantAndEquipmentAvailable = item.facts.usgaap.PropertyPlantAndEquipmentNet != null && item.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD != null && item.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD.Count > 0;
                                bool propertyPlantAndEquipmentBackupAvailable = item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization != null && item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization.units.USD != null && item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization.units.USD.Count > 0;
                                bool goodwillAvailable = item.facts.usgaap.Goodwill != null && item.facts.usgaap.Goodwill.units.USD != null && item.facts.usgaap.Goodwill.units.USD.Count > 0;
                                bool intangibleAssetsAvailable = item.facts.usgaap.FiniteLivedIntangibleAssetsNet != null && item.facts.usgaap.FiniteLivedIntangibleAssetsNet.units.USD != null && item.facts.usgaap.FiniteLivedIntangibleAssetsNet.units.USD.Count > 0;
                                bool trademarksAvailable = item.facts.usgaap.IndefiniteLivedTrademarks != null && item.facts.usgaap.IndefiniteLivedTrademarks.units.USD != null && item.facts.usgaap.IndefiniteLivedTrademarks.units.USD.Count > 0;
                                bool assetsAvailable = item.facts.usgaap.Assets != null && item.facts.usgaap.Assets.units.USD != null && item.facts.usgaap.Assets.units.USD.Count > 0;
                                bool longTermDebtAvailable = item.facts.usgaap.LongTermDebt != null && item.facts.usgaap.LongTermDebt.units.USD != null && item.facts.usgaap.LongTermDebt.units.USD.Count > 0;
                                bool longTermDebtBackupAvailable = item.facts.usgaap.LongTermDebtNoncurrent != null && item.facts.usgaap.LongTermDebtNoncurrent.units.USD != null && item.facts.usgaap.LongTermDebtNoncurrent.units.USD.Count > 0;
                                bool longTermNotesPayableAvailable = item.facts.usgaap.LongTermNotesPayable != null && item.facts.usgaap.LongTermNotesPayable.units.USD != null && item.facts.usgaap.LongTermNotesPayable.units.USD.Count > 0;
                                bool longTermLineOfCreditAvailable = item.facts.usgaap.LongTermLineOfCredit != null && item.facts.usgaap.LongTermLineOfCredit.units.USD != null && item.facts.usgaap.LongTermLineOfCredit.units.USD.Count > 0;
                                bool currentDebtAvailable = item.facts.usgaap.LongTermDebtCurrent != null && item.facts.usgaap.LongTermDebtCurrent.units.USD != null && item.facts.usgaap.LongTermDebtCurrent.units.USD.Count > 0;
                                bool currentDebtBackupAvailable = item.facts.usgaap.DebtCurrent != null && item.facts.usgaap.DebtCurrent.units.USD != null && item.facts.usgaap.DebtCurrent.units.USD.Count > 0;
                                bool currentNotesPayableAvailable = item.facts.usgaap.NotesPayableCurrent != null && item.facts.usgaap.NotesPayableCurrent.units.USD != null && item.facts.usgaap.NotesPayableCurrent.units.USD.Count > 0;
                                bool currentLineOfCreditAvailable = item.facts.usgaap.LinesOfCreditCurrent != null && item.facts.usgaap.LinesOfCreditCurrent.units.USD != null && item.facts.usgaap.LinesOfCreditCurrent.units.USD.Count > 0;
                                bool liabilitiesAvailable = item.facts.usgaap.Liabilities != null && item.facts.usgaap.Liabilities.units.USD != null && item.facts.usgaap.Liabilities.units.USD.Count > 0;
                                bool operatingIncomeAvailable = item.facts.usgaap.OperatingIncomeLoss != null && item.facts.usgaap.OperatingIncomeLoss.units.USD != null && item.facts.usgaap.OperatingIncomeLoss.units.USD.Count > 0;

                                // What is the most recent filed date?

                                if (cashAndCashEquivalentsAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD[^1].filed);
                                else if (cashAndCashEquivalentsBackupAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.Cash.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.Cash.units.USD[^1].filed);

                                if (currentAssetsAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.AssetsCurrent.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.AssetsCurrent.units.USD[^1].filed);

                                if (propertyPlantAndEquipmentAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD[^1].filed);
                                else if (propertyPlantAndEquipmentBackupAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization.units.USD[^1].filed);

                                if (goodwillAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.Goodwill.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.Goodwill.units.USD[^1].filed);

                                if (intangibleAssetsAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.FiniteLivedIntangibleAssetsNet.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.FiniteLivedIntangibleAssetsNet.units.USD[^1].filed);

                                if (trademarksAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.IndefiniteLivedTrademarks.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.IndefiniteLivedTrademarks.units.USD[^1].filed);

                                if (assetsAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.Assets.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.Assets.units.USD[^1].filed);

                                if (longTermDebtAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.LongTermDebt.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.LongTermDebt.units.USD[^1].filed);
                                else if (longTermDebtBackupAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.LongTermDebtNoncurrent.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.LongTermDebtNoncurrent.units.USD[^1].filed);
                                else if (longTermNotesPayableAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.LongTermNotesPayable.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.LongTermNotesPayable.units.USD[^1].filed);
                                else if (longTermLineOfCreditAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.LongTermLineOfCredit.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.LongTermLineOfCredit.units.USD[^1].filed);

                                if (currentDebtAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.LongTermDebtCurrent.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.LongTermDebtCurrent.units.USD[^1].filed);
                                else if (currentDebtBackupAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.DebtCurrent.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.DebtCurrent.units.USD[^1].filed);
                                else if (currentNotesPayableAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.NotesPayableCurrent.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.NotesPayableCurrent.units.USD[^1].filed);
                                else if (currentLineOfCreditAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.LinesOfCreditCurrent.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.LinesOfCreditCurrent.units.USD[^1].filed);

                                if (liabilitiesAvailable && (filedDate == null || DateTime.Parse(item.facts.usgaap.Liabilities.units.USD[^1].filed) > filedDate))
                                    filedDate = DateTime.Parse(item.facts.usgaap.Liabilities.units.USD[^1].filed);

                                if (filedDate != null)
                                {
                                    if (cashAndCashEquivalentsAvailable && DateTime.Parse(item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD[^1].filed) == filedDate)
                                        cashAndCashEquivalents.val = item.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD[^1].val;
                                    else if (cashAndCashEquivalentsBackupAvailable && DateTime.Parse(item.facts.usgaap.Cash.units.USD[^1].filed) == filedDate)
                                        cashAndCashEquivalents.val = item.facts.usgaap.Cash.units.USD[^1].val;

                                    if (currentAssetsAvailable && DateTime.Parse(item.facts.usgaap.AssetsCurrent.units.USD[^1].filed) == filedDate)
                                        currentAssets.val = item.facts.usgaap.AssetsCurrent.units.USD[^1].val;

                                    if (propertyPlantAndEquipmentAvailable && (DateTime.Parse(item.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD[^1].filed) == filedDate))
                                        propertyPlantAndEquipment.val = item.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD[^1].val;
                                    else if (propertyPlantAndEquipmentBackupAvailable && DateTime.Parse(item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization.units.USD[^1].filed) == filedDate)
                                        propertyPlantAndEquipment.val = item.facts.usgaap.PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization.units.USD[^1].val;

                                    if (goodwillAvailable && DateTime.Parse(item.facts.usgaap.Goodwill.units.USD[^1].filed) == filedDate)
                                        goodwill.val = item.facts.usgaap.Goodwill.units.USD[^1].val;

                                    if (intangibleAssetsAvailable && DateTime.Parse(item.facts.usgaap.FiniteLivedIntangibleAssetsNet.units.USD[^1].filed) == filedDate)
                                        intangibleAssets.val = item.facts.usgaap.FiniteLivedIntangibleAssetsNet.units.USD[^1].val;

                                    if (trademarksAvailable && DateTime.Parse(item.facts.usgaap.IndefiniteLivedTrademarks.units.USD[^1].filed) == filedDate)
                                        trademarks.val = item.facts.usgaap.IndefiniteLivedTrademarks.units.USD[^1].val;

                                    if (assetsAvailable && DateTime.Parse(item.facts.usgaap.Assets.units.USD[^1].filed) == filedDate)
                                        assets.val = item.facts.usgaap.Assets.units.USD[^1].val;

                                    if (longTermDebtAvailable && DateTime.Parse(item.facts.usgaap.LongTermDebt.units.USD[^1].filed) == filedDate)
                                        longTermDebt.val = item.facts.usgaap.LongTermDebt.units.USD[^1].val;
                                    else if (longTermDebtBackupAvailable && DateTime.Parse(item.facts.usgaap.LongTermDebtNoncurrent.units.USD[^1].filed) == filedDate)
                                        longTermDebt.val = item.facts.usgaap.LongTermDebtNoncurrent.units.USD[^1].val;
                                    else
                                    {
                                        if (longTermNotesPayableAvailable && DateTime.Parse(item.facts.usgaap.LongTermNotesPayable.units.USD[^1].filed) == filedDate)
                                            longTermDebt.val += item.facts.usgaap.LongTermNotesPayable.units.USD[^1].val;

                                        if (longTermLineOfCreditAvailable && DateTime.Parse(item.facts.usgaap.LongTermLineOfCredit.units.USD[^1].filed) == filedDate)
                                            longTermDebt.val += item.facts.usgaap.LongTermLineOfCredit.units.USD[^1].val;
                                    }

                                    if (currentDebtAvailable && DateTime.Parse(item.facts.usgaap.LongTermDebtCurrent.units.USD[^1].filed) == filedDate)
                                        currentDebt.val = item.facts.usgaap.LongTermDebtCurrent.units.USD[^1].val;
                                    else if (currentDebtBackupAvailable && DateTime.Parse(item.facts.usgaap.DebtCurrent.units.USD[^1].filed) == filedDate)
                                        currentDebt.val = item.facts.usgaap.DebtCurrent.units.USD[^1].val;
                                    else
                                    {
                                        if (currentNotesPayableAvailable && DateTime.Parse(item.facts.usgaap.NotesPayableCurrent.units.USD[^1].filed) == filedDate)
                                            currentDebt.val += item.facts.usgaap.NotesPayableCurrent.units.USD[^1].val;

                                        if (currentLineOfCreditAvailable && DateTime.Parse(item.facts.usgaap.LinesOfCreditCurrent.units.USD[^1].filed) == filedDate)
                                            currentDebt.val += item.facts.usgaap.LinesOfCreditCurrent.units.USD[^1].val;
                                    }

                                    if (liabilitiesAvailable && DateTime.Parse(item.facts.usgaap.Liabilities.units.USD[^1].filed) == filedDate)
                                        liabilities.val = item.facts.usgaap.Liabilities.units.USD[^1].val;

                                    if (operatingIncomeAvailable)
                                    {
                                        var lastOpIncome = item.facts.usgaap.OperatingIncomeLoss.units.USD[^1];
                                        var lastOpIncomeDate = DateTime.Parse(lastOpIncome.filed);

                                        if (cik == "14177")
                                        {

                                        }

                                        // Do not consider operating income that's too old (before current year and last year).
                                        if (lastOpIncomeDate.Year >= DateTime.Now.Year - 1)
                                        {
                                            var lastQuarterYearToDateData = item.facts.usgaap.OperatingIncomeLoss.units.USD
                                                .LastOrDefault(c => c.fp == lastOpIncome.fp && c.end == lastOpIncome.end && c.frame == null);

                                            if (lastQuarterYearToDateData != null)
                                            {
                                                var lastYearlyData = item.facts.usgaap.OperatingIncomeLoss.units.USD.LastOrDefault(c => c.fp == "FY");
                                                if (lastYearlyData != null)
                                                {
                                                    var lastQuarterYearToDateDataStart = DateTime.Parse(lastQuarterYearToDateData.start).AddMonths(-12);
                                                    var withinDays = 10;
                                                    var lastYearQuarterYearToDateData = item.facts.usgaap.OperatingIncomeLoss.units.USD
                                                        .LastOrDefault(c => c.fp == lastOpIncome.fp
                                                            && c.start != lastQuarterYearToDateData.start
                                                            && DateTime.Parse(c.start) > lastQuarterYearToDateDataStart.AddDays(-withinDays)
                                                            && DateTime.Parse(c.start) < lastQuarterYearToDateDataStart.AddDays(withinDays)
                                                            && c.frame == null);

                                                    if (lastQuarterYearToDateData != null && lastYearQuarterYearToDateData != null)
                                                    {
                                                        // Do the calculation:
                                                        // last quarter up to date operating income
                                                        // + last 10-K operating income
                                                        // - last year's same quarter up to date operating income
                                                        // = last twelve months operating income.
                                                        operatingIncome.val = lastQuarterYearToDateData.val + lastYearlyData.val - lastYearQuarterYearToDateData.val;
                                                        operatingIncome.filed = lastQuarterYearToDateData.filed;
                                                    }
                                                    else
                                                        operatingIncome = lastYearlyData;
                                                }
                                                else
                                                    operatingIncome = lastQuarterYearToDateData;
                                            }
                                            else
                                            {
                                                var lastYearlyData = item.facts.usgaap.OperatingIncomeLoss.units.USD.Last(c => c.fp == "FY");
                                                if (lastYearlyData != null)
                                                    operatingIncome = lastYearlyData;
                                            }
                                        }
                                    }

                                    var ticker = tickerData[2].ToString();
                                    var company = context.Companies.FirstOrDefault(c => c.CIK == cik);
                                    company ??= new Company
                                    {
                                        CIK = cik,
                                        Ticker = ticker,
                                        Exchange = tickerData[3]?.ToString(),
                                        CompanyName = tickerData[1]?.ToString()
                                    };

                                    var employedCapital = currentAssets.val + propertyPlantAndEquipment.val;
                                    company.CashAndCashEquivalents = cashAndCashEquivalents.val;
                                    company.CurrentAssets = currentAssets.val;
                                    company.PropertyPlantAndEquipment = propertyPlantAndEquipment.val;
                                    company.Assets = assets.val;
                                    company.TotalDebt = longTermDebt.val + currentDebt.val;
                                    company.Liabilities = liabilities.val;
                                    company.OperatingIncome = operatingIncome.val;
                                    company.NetCurrentAssets = currentAssets.val - liabilities.val;
                                    company.TangibleAssets = assets.val - goodwill.val - intangibleAssets.val - trademarks.val;
                                    company.EmployedCapital = employedCapital;
                                    company.ReturnOnEmployedCapital = (float)(employedCapital != 0 ? operatingIncome.val / employedCapital : 0);
                                    company.LastFilingDate = filedDate.Value;

                                    if (fmpConfig != null && (fmpConfig.LastDay == null || fmpConfig.LastDay < DateTime.Today))
                                    {
                                        if (_fmpCalls < fmpConfig.MaxRequestsPerDay)
                                        {
                                            if (_currentBatch >= fmpConfig.LastBatch
                                                && operatingIncome.filed != null
                                                && (company.LastMarketCapitalizationDate == null
                                                || company.LastMarketCapitalizationDate != null && company.LastMarketCapitalizationDate.Value.AddSeconds(fmpConfig.MinimumTimeinSecondsToUpdateMarketCapitalizations) < DateTime.Now))
                                            {
                                                fmpTasks.Add(fmpApiClient.CompanyValuation.GetQuoteAsync(ticker));
                                                _fmpCalls++;
                                            }
                                        }
                                        else
                                        {
                                            fmpConfig.LastBatch = _currentBatch.Value;
                                            fmpConfig.LastDay = DateTime.Today;
                                            _fmpCalls = 0;
                                        }
                                    }

                                    if (!context.Companies.Any(c => c.CIK == cik))
                                    {
                                        context.Companies.Add(company);
                                    }

                                    await context.SaveChangesAsync(stoppingToken);
                                }
                            }
                        }
                    }

                    _logger.LogInformation("Updating market capitalizations...");

                    var fmpResults = await Task.WhenAll(fmpTasks);
                    foreach (var response in fmpResults)
                    {
                        if (!response.HasError)
                        {
                            var company = context.Companies.Single(c => c.Ticker == response.Data.Symbol);
                            if (company != null)
                            {
                                var marketCapitalization = (decimal)response.Data.MarketCap;
                                var enterpriseValue = marketCapitalization + company.TotalDebt - company.CashAndCashEquivalents;
                                company.CompanyName = response.Data.Name;
                                company.LastMarketCapitalization = marketCapitalization;
                                company.EnterpriseValue = enterpriseValue;
                                company.OperatingIncomeToEnterpriseValue = (float)(enterpriseValue != 0 ? company.OperatingIncome / enterpriseValue : 0);
                                company.LastMarketCapitalizationDate = DateTime.Now;

                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            _logger.LogError("Error occured while calling FMP API.\n{error}", response.Error);
                        }
                    }

                    _currentBatch++;
                    if ((_currentBatch - 1) * batchSize > files.Count)
                    {
                        _currentBatch = 1;
                    }
                }
            }
            
            await Task.Delay(1000, stoppingToken);
        }

        private static async Task<T> ReadJsonFileAsync<T>(string path)
        {
            // Adjust this to be a reasonably sized multiple of 4096 that's at least larger than any file you'll process.
            const int asyncFileStreamBufferSize = 1 * 1024 * 8192;

            using FileStream fs = new(path: path, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: asyncFileStreamBufferSize, useAsync: true);
            using StreamReader rdr = new(fs);
            string fileText = await rdr.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(fileText);
        }
    }
}
