using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SecImporter;
using Shared;
using Shared.Models;
using System.Diagnostics;
using System.Text.Json;

var stopwatch = new Stopwatch();
var total = new Stopwatch();

stopwatch.Start();
total.Start();

var companiesFolder = @"C:\Users\aldol\source\repos\MagicFormulaApp\MagicFormulaApp\Magic Formula App\SecImporter\companyfacts";
var companyFiles = Directory.GetFiles(companiesFolder);

stopwatch.Stop();

Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds} ms\tFound {companyFiles.Length} company files.");

if (companyFiles.Length > 0)
{
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    Settings settings = configuration.GetRequiredSection("Settings").Get<Settings>();

    // As the time it takes for the entire folder to be imported can be very long,
    // you can apply "SecImporterForceQuit":"yes" at the appsettings.json file in realtime to halt the process.
    var quit = false;
    var curr = 0;
    var currInBatch = 0;
    var imported = 0;
    var batch = 10;
    var connectionString = configuration.GetConnectionString(settings.DatabaseProvider);
    CompanyData db = settings.DatabaseProvider switch
    {
        DatabaseProvider.SqlServer => new SqlServerCompanyData(),
        DatabaseProvider.Sqlite => new SqliteCompanyData(),
        DatabaseProvider.Postgres => new PostgresCompanyData(),
        _ => throw new Exception($"Unsupported database provider: {settings.DatabaseProvider}"),
    };

    do
    {
        stopwatch.Restart();

        var companyData = companyFiles[curr];
        using StreamReader reader = new(companyData);
        string jsonString = reader.ReadToEnd();
        var root = JsonSerializer.Deserialize<Root>(jsonString);

        if (root.cik != null && root.entityName != null && root.facts.usgaap != null
            && root.facts.dei != null && root.facts.dei.EntityPublicFloat != null && root.facts.dei.EntityPublicFloat.units.USD != null
            && root.facts.usgaap.AssetsCurrent != null && root.facts.usgaap.AssetsCurrent.units.USD != null
            && root.facts.usgaap.CashAndCashEquivalentsAtCarryingValue != null && root.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD != null
            && root.facts.usgaap.PropertyPlantAndEquipmentNet != null && root.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD != null
            && ((root.facts.usgaap.LongTermDebt != null
                    && root.facts.usgaap.LongTermDebt.units.USD != null)
                    || (root.facts.usgaap.LongTermDebtAndCapitalLeaseObligations != null
                        && root.facts.usgaap.LongTermDebtAndCapitalLeaseObligations.units.USD != null))
            && ((root.facts.usgaap.OperatingIncomeLoss != null
                    && root.facts.usgaap.OperatingIncomeLoss.units.USD != null)
                    || (root.facts.usgaap.IncomeLossFromContinuingOperations != null
                        && root.facts.usgaap.IncomeLossFromContinuingOperations.units.USD != null)))
        {
            var cik = root.cik.ToString();
            if (!db.Companies.Any(c => c.CIK == cik))
            {
                // Data to calculate enterprise value.
                decimal? marketCapitalization = default;
                decimal? cashAndCashEquivalents = default;
                decimal? totalDebt = default;

                // Data needed to calculate return on assets.
                decimal? assetsCurrent = default;
                decimal? netPropertyPlantAndEquipment = default;
                decimal? operatingIncome = default;

                var lastMarketCapitalization = root.facts.dei.EntityPublicFloat.units.USD
                    .OrderByDescending(c => c.filed)
                    .FirstOrDefault();

                // Frame holds the year and quarter filing info.
                var filed = "";
                if (lastMarketCapitalization != null)
                {
                    filed = lastMarketCapitalization.filed;
                    marketCapitalization = lastMarketCapitalization.val;
                }

                var lastAssetsCurrent = root.facts.usgaap.AssetsCurrent.units.USD
                    .OrderByDescending(c => c.filed)
                    .FirstOrDefault();

                if (lastAssetsCurrent != null && lastAssetsCurrent.filed == filed)
                {
                    assetsCurrent = lastAssetsCurrent.val;
                }

                var lastCashAndCashEquivalents = root.facts.usgaap.CashAndCashEquivalentsAtCarryingValue.units.USD
                    .OrderByDescending(c => c.filed)
                    .FirstOrDefault();

                if (lastCashAndCashEquivalents != null && lastCashAndCashEquivalents.filed == filed)
                {
                    cashAndCashEquivalents = lastCashAndCashEquivalents.val;
                }

                var lastPropertyPlantAndEquipment = root.facts.usgaap.PropertyPlantAndEquipmentNet.units.USD
                    .OrderByDescending(c => c.filed)
                    .FirstOrDefault();

                if (lastPropertyPlantAndEquipment != null && lastPropertyPlantAndEquipment.filed == filed)
                {
                    netPropertyPlantAndEquipment = lastCashAndCashEquivalents.val;
                }

                var lastDebt = root.facts.usgaap.LongTermDebt != null
                    && root.facts.usgaap.LongTermDebt.units.USD != null
                    ? root.facts.usgaap.LongTermDebt.units.USD
                        .OrderByDescending(c => c.filed)
                        .FirstOrDefault()
                    : root.facts.usgaap.LongTermDebtAndCapitalLeaseObligations.units.USD
                        .OrderByDescending(c => c.filed)
                        .FirstOrDefault();

                if (lastDebt != null && lastDebt.filed == filed)
                {
                    totalDebt = lastDebt.val;
                }

                var lastOperatingIncome = root.facts.usgaap.OperatingIncomeLoss != null
                    && root.facts.usgaap.OperatingIncomeLoss.units.USD != null
                    ? root.facts.usgaap.OperatingIncomeLoss.units.USD
                        .OrderByDescending(c => c.filed)
                        .FirstOrDefault()
                    : root.facts.usgaap.IncomeLossFromContinuingOperations.units.USD
                        .OrderByDescending(c => c.filed)
                        .FirstOrDefault();

                if (lastOperatingIncome != null && lastOperatingIncome.filed == filed)
                {
                    operatingIncome = lastOperatingIncome.val;
                }

                if (marketCapitalization.HasValue && cashAndCashEquivalents.HasValue && totalDebt.HasValue && assetsCurrent.HasValue && netPropertyPlantAndEquipment.HasValue && operatingIncome.HasValue)
                {
                    var enterpriseValue = marketCapitalization + totalDebt - cashAndCashEquivalents;
                    var employedCapital = assetsCurrent + netPropertyPlantAndEquipment;
                    var company = new Company
                    {
                        CIK = cik,
                        CompanyName = root.entityName,
                        LastMarketCapitalization = marketCapitalization.Value,
                        LastTotalDebt = totalDebt.Value,
                        LastCashAndCashEquivalents = cashAndCashEquivalents.Value,
                        LastNetCurrentAssets = assetsCurrent.Value,
                        LastNetPropertyPlantAndEquipment = netPropertyPlantAndEquipment.Value,
                        LastOperatingIncome = operatingIncome.Value,
                        LastEnterpriseValue = enterpriseValue.Value,
                        LastEmployedCapital = employedCapital.Value,
                        LastOperatingIncomeToEnterpriseValue = (float)(operatingIncome / enterpriseValue),
                        LastReturnOnAssets = (float)(operatingIncome / employedCapital),
                        LastFilingDate = DateTime.Parse(lastMarketCapitalization.filed)
                    };

                    db.Add(company);
                    db.SaveChanges();

                    Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds} ms\t\tImported {companyData}.");

                    currInBatch++;
                    imported++;

                    if (currInBatch % batch == 0)
                    {
                        using StreamReader configReader = new("appsettings.json");
                        jsonString = configReader.ReadToEnd();
                        var appSettings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                        quit = !string.IsNullOrEmpty(appSettings.Settings.SecImporterForceQuit) && appSettings.Settings.SecImporterForceQuit.Equals("yes", StringComparison.OrdinalIgnoreCase);
                    }

                    if (currInBatch == batch)
                    {
                        currInBatch = 0;
                    }
                }
            }
        }

        stopwatch.Stop();

        curr++;
    }
    while (!quit && curr < companyFiles.Length);

    total.Stop();

    Console.WriteLine($"{total.ElapsedMilliseconds} ms\t\tWent through {curr} files (batch of {batch}) and importerd {imported} files to the database...");
    Console.WriteLine("Press any key to quit.");
    Console.ReadLine();
}
