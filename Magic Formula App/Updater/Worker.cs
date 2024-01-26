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
        private JsonElement _tickerData;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_tickerData.ValueKind == JsonValueKind.Undefined)
                {
                    _logger.LogInformation("No ticker data available. Fetching ticker data...");
                    var httpClient = _httpClientFactory.CreateClient("SecClient");

                    var httpResponseMessage = await httpClient.GetAsync(_configuration["TickerDataDownloadUrlLocation"], stoppingToken);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var jsonString = await httpResponseMessage.Content.ReadAsStringAsync(stoppingToken);
                        using var document = JsonDocument.Parse(jsonString);
                        _tickerData = document.RootElement.Clone();
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

                    // TODO Find a way to process these one by one to boost performance (watch out for memory leakage).
                    List<Task<JsonElement>> companyDocumentTasks = filesToRead
                        .Select(fi => ReadCompanyDocumentAsync(fi.FullName))
                        .ToList();

                    var companyDocuments = await Task.WhenAll(companyDocumentTasks);

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

                    foreach (var document in companyDocuments)
                    {
                        if (document.FindProperty("cik", out var cik) && document.FindProperty("facts.us-gaap", out _))
                        {
                            var cikInt = default(int?);
                            var cikString = default(string);
                            switch (cik.ValueKind)
                            {
                                case JsonValueKind.String:
                                    cikString = cik.GetString();
                                    cikInt = int.Parse(cikString);
                                break;
                                case JsonValueKind.Number:
                                    cikInt = cik.GetInt32();
                                    cikString = cikInt.ToString();
                                break;
                            }

                            if (_tickerData.FindProperty("data", out var tickerData) && tickerData.EnumerateArray().FirstOrDefault(c => c[0].GetInt32() == cikInt).ValueKind != JsonValueKind.Undefined)
                            {
                                // sometimes these fields are null so no filed is available.
                                // they should be considered with value 0 in these situations.
                                // and filed shouldn't matter.
                                // if they're non-null, filed date matters because i want the latest financial data snapshot to have the same filing date.
                                // If the date of any recent financial data is old, it simply means value should be treated as zero as it has had no update.

                                var fields = new Dictionary<string, CompanyField>()
                                {
                                    {
                                        "CashAndCashEquivalents",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() { { "CashAndCashEquivalentsAtCarryingValue", default } },
                                            BackupKeyDate = new Dictionary<string, DateTime?>() { { "Cash", default } }
                                        }
                                    },
                                    {
                                        "CurrentAssets",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() { { "AssetsCurrent", default } }
                                        }
                                    },
                                    {
                                        "PropertyPlantAndEquipment",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() { { "PropertyPlantAndEquipment", default } },
                                            BackupKeyDate = new Dictionary<string, DateTime?>() { { "PropertyPlantAndEquipmentAndFinanceLeaseRightOfUseAssetAfterAccumulatedDepreciationAndAmortization", default } }
                                        }
                                    },
                                    {
                                        "IntangibleAssets",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() { { "PropertyPlantAndEquipment", default }, { "FiniteLivedIntangibleAssetsNet", default }, { "IndefiniteLivedTrademarks", default } }
                                        }
                                    },
                                    {
                                        "Assets",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() { { "Assets", default } }
                                        }
                                    },
                                    {
                                        "Debt",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() {
                                                { "LongTermDebt", default },
                                                { "LongTermDebtNoncurrent", default } ,
                                                { "LongTermNotesPayable", default } ,
                                                { "LongTermLineOfCredit", default } ,
                                                { "LongTermDebtCurrent", default } ,
                                                { "DebtCurrent", default },
                                                { "NotesPayableCurrent", default },
                                                { "LinesOfCreditCurrent", default }
                                            }
                                        }
                                    },
                                    {
                                        "Liabilities",
                                        new CompanyField
                                        {
                                            KeyDate = new Dictionary<string, DateTime?>() { { "Liabilities", default } },
                                            BackupKeyDate = new Dictionary<string, DateTime?>() {
                                                { "AccountsPayableCurrent", default },
                                                { "AccruedLiabilitiesCurrent", default },
                                                { "EmployeeRelatedLiabilitiesCurrent", default },
                                                { "AccruedEmployeeBenefitsAndBonus", default },
                                                { "AccruedIncomeTaxesCurrent", default },
                                                { "ContractWithCustomerLiabilityCurrent", default },
                                                { "OperatingLeaseLiabilityCurrent", default },
                                                { "OtherAccruedLiabilitiesCurrent", default },
                                                { "ConvertibleNotesPayable", default },
                                                { "OperatingLeaseLiabilityNoncurrent", default },
                                                { "DeferredRevenueAndCreditsNoncurrent", default },
                                                { "DeferredIncomeTaxLiabilitiesNet", default },
                                                { "LongTermDebt", default },
                                                { "LongTermDebtNoncurrent", default } ,
                                                { "LongTermNotesPayable", default } ,
                                                { "LongTermLineOfCredit", default } ,
                                                { "LongTermDebtCurrent", default } ,
                                                { "DebtCurrent", default },
                                                { "NotesPayableCurrent", default },
                                                { "LinesOfCreditCurrent", default },
                                                { "PostemploymentBenefitsLiabilityNoncurrent", default },
                                                { "OtherLiabilities", default },
                                                { "OtherLiabilitiesNoncurrent", default }
                                            }
                                        }
                                    },
                                    {
                                        "OperatingIncome",
                                        new CompanyField()
                                    }
                                };

                                // What is the most recent filing date?
                                var filingDate = default(DateTime?);
                                foreach (var field in fields)
                                {
                                    // Get the "filed" date for each key in the document.
                                    if (field.Value.KeyDate != null)
                                        foreach (var keyDate in field.Value.KeyDate)
                                            if (document.TryGetLastDate("filed", $"facts.us-gaap.{keyDate.Key}.units.USD", out var date))
                                            {
                                                field.Value.KeyDate[keyDate.Key] = date;
                                                if (field.Value.LastFilingDate == null || date > field.Value.LastFilingDate)
                                                    field.Value.LastFilingDate = date;
                                            }

                                    // If no date is found for the keys, try the backup keys.
                                    if (field.Value.LastFilingDate == null && field.Value.BackupKeyDate != null)
                                        foreach (var keyDate in field.Value.BackupKeyDate)
                                            if (document.TryGetLastDate("filed", $"facts.us-gaap.{keyDate.Key}.units.USD", out var date))
                                            {
                                                field.Value.BackupKeyDate[keyDate.Key] = date;
                                                if (field.Value.LastFilingDate == null || date > field.Value.LastFilingDate)
                                                    field.Value.LastFilingDate = date;
                                            }

                                    // If the most recent filing date for this key is newer than for the other keys, update filing date.
                                    if (filingDate == null || (field.Value.LastFilingDate != null && field.Value.LastFilingDate > filingDate))
                                        filingDate = field.Value.LastFilingDate;
                                }

                                if (filingDate != null)
                                {
                                    // Add all keys values for each field of interest, if the filing date matches the most recent one.
                                    foreach (var field in fields)
                                    {
                                        if (field.Value.KeyDate != null)
                                            foreach (var keyDate in field.Value.KeyDate)
                                                if (keyDate.Value == filingDate)
                                                    if (document.TryGetLastDecimal("val", $"facts.us-gaap.{keyDate.Key}.units.USD", out var value))
                                                    {
                                                        if (field.Value.Value == default)
                                                            field.Value.Value = 0;

                                                        if (value.HasValue)
                                                            field.Value.Value += value;
                                                    }

                                        // If value is still NULL, try the backup keys.
                                        if (field.Value.Value == default && field.Value.BackupKeyDate != null)
                                            foreach (var keyDate in field.Value.BackupKeyDate)
                                                if (keyDate.Value == filingDate)
                                                    if (document.TryGetLastDecimal("val", $"facts.us-gaap.{keyDate.Key}.units.USD", out var value))
                                                    {
                                                        if (field.Value.Value == default)
                                                            field.Value.Value = 0;

                                                        if (value.HasValue)
                                                            field.Value.Value += value;
                                                    }
                                    }

                                    if (document.FindProperty("facts.us-gaap.OperatingIncomeLoss.units.USD", out var operatingIncomeElement))
                                    {
                                        var operatingIncomes = operatingIncomeElement.EnumerateArray();
                                        if (operatingIncomes.Any())
                                        {
                                            var lastOpIncome = operatingIncomes.Last();
                                            var lastOpIncomeDate = lastOpIncome.GetValue("filed").GetDateTime();
                                            var lastOpIncomeFp = lastOpIncome.GetValue("fp").GetString();
                                            var lastOpIncomeStart = lastOpIncome.GetValue("start").GetDateTime();
                                            var lastOpIncomeEnd = lastOpIncome.GetValue("end").GetDateTime();

                                            // Do not consider operating income that's too old (before current year and last year).
                                            try
                                            {
                                                if (lastOpIncomeDate.Year >= DateTime.Now.Year - 1)
                                                {
                                                    var lastQuarterYearToDateData = operatingIncomes
                                                        .LastOrDefault(c => c.GetValue("fp").GetString() == lastOpIncomeFp
                                                            && c.GetValue("end").GetDateTime() == lastOpIncomeEnd
                                                            && c.GetValue("frame").ValueKind == JsonValueKind.Undefined);

                                                    if (lastQuarterYearToDateData.ValueKind != JsonValueKind.Undefined)
                                                    {
                                                        var lastYearlyData = operatingIncomes.LastOrDefault(c => c.GetValue("fp").GetString() == "FY");
                                                        if (lastYearlyData.ValueKind != JsonValueKind.Undefined)
                                                        {
                                                            var lastQuarterYearToDateDataStart = lastQuarterYearToDateData.GetValue("start").GetDateTime().AddMonths(-12);
                                                            var withinDays = 10;
                                                            var lastYearQuarterYearToDateData = operatingIncomes
                                                                .LastOrDefault(c => c.GetValue("fp").GetString() == lastOpIncomeFp
                                                                    && c.GetValue("start").GetDateTime() != lastOpIncomeStart
                                                                    && c.GetValue("start").GetDateTime() > lastQuarterYearToDateDataStart.AddDays(-withinDays)
                                                                    && c.GetValue("start").GetDateTime() < lastQuarterYearToDateDataStart.AddDays(withinDays)
                                                                    && c.GetValue("frame").ValueKind == JsonValueKind.Undefined);

                                                            if (lastYearQuarterYearToDateData.ValueKind != JsonValueKind.Undefined)
                                                            {
                                                                // Do the calculation:
                                                                // last quarter up to date operating income
                                                                // + last 10-K operating income
                                                                // - last year's same quarter up to date operating income
                                                                // = last twelve months operating income.
                                                                fields["OperatingIncome"].Value = lastQuarterYearToDateData.GetValue("val").GetDecimal()
                                                                    + lastYearlyData.GetValue("val").GetDecimal()
                                                                    - lastYearQuarterYearToDateData.GetValue("val").GetDecimal();

                                                                fields["OperatingIncome"].LastFilingDate = lastQuarterYearToDateData.GetValue("filed").GetDateTime();

                                                            }
                                                            else
                                                            {
                                                                fields["OperatingIncome"].Value = lastYearlyData.GetValue("val").GetDecimal();
                                                                fields["OperatingIncome"].LastFilingDate = lastYearlyData.GetValue("filed").GetDateTime();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            fields["OperatingIncome"].Value = lastQuarterYearToDateData.GetValue("val").GetDecimal();
                                                            fields["OperatingIncome"].LastFilingDate = lastQuarterYearToDateData.GetValue("filed").GetDateTime();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var lastYearlyData = operatingIncomes.LastOrDefault(c => c.GetValue("fp").GetString() == "FY");
                                                        if (lastYearlyData.ValueKind != JsonValueKind.Undefined)
                                                        {
                                                            fields["OperatingIncome"].Value = lastYearlyData.GetValue("val").GetDecimal();
                                                            fields["OperatingIncome"].LastFilingDate = lastYearlyData.GetValue("filed").GetDateTime();
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError("An error occurred when updating operating income for cik: {cik}\n{ex}", cikString, ex);
                                            }
                                        }
                                    }

                                    var ticker = tickerData.EnumerateArray().FirstOrDefault(c => c[0].GetInt32() == cikInt).Deserialize<object[]>();
                                    var tickerSymbol = ticker[2].ToString();
                                    var tickerCompanyName = ticker[1].ToString();
                                    var tickerExchange = ticker[3]?.ToString();
                                    var company = context.Companies.FirstOrDefault(c => c.CIK == cikString);
                                    company ??= new Company
                                    {
                                        CIK = cikString,
                                        Ticker = tickerSymbol,
                                        Exchange = tickerCompanyName,
                                        CompanyName = tickerExchange
                                    };

                                    company.CashAndCashEquivalents = fields["CashAndCashEquivalents"].Value ?? 0;
                                    company.CurrentAssets = fields["CurrentAssets"].Value ?? 0;
                                    company.PropertyPlantAndEquipment = fields["PropertyPlantAndEquipment"].Value ?? 0;
                                    company.IntangibleAssets = fields["IntangibleAssets"].Value ?? 0;
                                    company.Assets = fields["Assets"].Value ?? 0;
                                    company.Debt = fields["Debt"].Value ?? 0;
                                    company.Liabilities = fields["Liabilities"].Value ?? 0;
                                    company.OperatingIncome = fields["OperatingIncome"].Value ?? 0;
                                    company.LastFilingDate = filingDate.Value;

                                    if (fmpConfig != null && (fmpConfig.LastDay == null || fmpConfig.LastDay < DateTime.Today))
                                    {
                                        if (_fmpCalls < fmpConfig.MaxRequestsPerDay)
                                        {
                                            if (_currentBatch >= fmpConfig.LastBatch
                                                && fields["OperatingIncome"].Value != default
                                                && (company.LastMarketCapitalizationDate == null
                                                    || company.LastMarketCapitalizationDate != null && company.LastMarketCapitalizationDate.Value.AddSeconds(fmpConfig.MinimumTimeinSecondsToUpdateMarketCapitalizations) < DateTime.Now))
                                            {
                                                fmpTasks.Add(fmpApiClient.CompanyValuation.GetQuoteAsync(tickerSymbol));
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

                                    if (!context.Companies.Any(c => c.CIK == cikString))
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
                                company.CompanyName = response.Data.Name;
                                company.LastMarketCapitalization = marketCapitalization;
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
        }

        private static async Task<JsonElement> ReadCompanyDocumentAsync(string path)
        {
            // Adjust this to be a reasonably sized multiple of 4096 that's at least larger than any file you'll process.
            const int asyncFileStreamBufferSize = 1 * 1024 * 8192;

            using FileStream fs = new(path: path, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: asyncFileStreamBufferSize, useAsync: true);
            using StreamReader rdr = new(fs);
            string fileText = await rdr.ReadToEndAsync();
            using var document = JsonDocument.Parse(fileText);
            return document.RootElement.Clone();
        }
    }
}
