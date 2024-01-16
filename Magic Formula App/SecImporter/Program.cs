using SecImporter;
using SecImporter.Models;
using System.Diagnostics;
using System.Text.Json;

var stopwatch = new Stopwatch();

stopwatch.Start();

var companyFiles = Directory.GetFiles(@"location\to\companyfacts");

stopwatch.Stop();

Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds} ms\tFound {companyFiles.Length} company files.");

if (companyFiles.Length > 0)
{
    // As the time it takes for the entire folder to be imported can be very long, you can apply "stop":"yes" at the config file.
    var stop = false;
    var curr = 0;
    var currInBatch = 0;
    var batch = 10;
    //var dateFormat = "yyyy-MM-dd";
    var db = new CompanyData();

    do
    {
        stopwatch.Restart();

        var companyData = companyFiles[curr];
        using StreamReader reader = new(companyData);
        string jsonString = reader.ReadToEnd();
        var root = JsonSerializer.Deserialize<Root>(jsonString);
        if (root.cik != null && root.entityName != null && root.facts.dei != null)
        {
            var cik = root.cik.ToString();
            var commonSharesOustanding = new List<Share>();
            if (root.facts.dei.EntityCommonStockSharesOutstanding != null)
            {
                commonSharesOustanding = root.facts.dei.EntityCommonStockSharesOutstanding.units.shares;
            }

            db.Add(new Company
            {
                CIK = cik,
                CompanyName = root.entityName,
                CommonSharesOutstanding = commonSharesOustanding
                    .Select(c => new CommonSharesOutstanding
                    {
                        End = DateTime.Parse(c.end),
                        FiscalYear = c.fy.Value,
                        FiscalPart = c.fp,
                        Form = c.form,
                        FileDate = DateTime.Parse(c.filed)
                    })
                    .ToList()
            });

            db.SaveChanges();

            stopwatch.Stop();

            Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds} ms\t\tImported {companyData}.");

            currInBatch++;

            if (currInBatch % batch == 0)
            {
                using StreamReader configReader = new("config.json");
                jsonString = configReader.ReadToEnd();
                var config = JsonSerializer.Deserialize<Config>(jsonString);
                stop = !string.IsNullOrEmpty(config.Stop) && config.Stop.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }

            if (currInBatch == batch)
            {
                currInBatch = 0;
            }
        }

        curr++;
    }
    while (!stop && curr < companyFiles.Length);

    Console.WriteLine($"\n...Went through {curr} files (batch of {batch})...");
    Console.ReadLine();
}
