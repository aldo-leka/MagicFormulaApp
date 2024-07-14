using System.IO.Compression;

namespace MagicFormulaApp.Importer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 1. Download
                var companyFactsZipFile = new FileInfo(_configuration["CompanyFactsDownloadFile"]);

                _logger.LogInformation("Checking if it is necessary to download {companyFactsUrl}.", _configuration["BaseAddress"] + _configuration["CompanyFactsDownloadUrlLocation"]);

                if (!companyFactsZipFile.Exists || (companyFactsZipFile.Exists && companyFactsZipFile.CreationTime.Date < DateTime.Today))
                {
                    _logger.LogInformation("Last date {companyFactsLocation} was downloaded was {date}.", companyFactsZipFile.FullName, companyFactsZipFile.CreationTime.Date);
                    var httpClient = _httpClientFactory.CreateClient("SecClient");

                    using (HttpResponseMessage response = httpClient.GetAsync(_configuration["CompanyFactsDownloadUrlLocation"], HttpCompletionOption.ResponseHeadersRead, stoppingToken).Result)
                    {
                        response.EnsureSuccessStatusCode();

                        var total = response.Content.Headers.ContentLength;

                        using Stream contentStream = await response.Content.ReadAsStreamAsync(stoppingToken);
                        var totalRead = 0L;
                        var totalReads = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        using FileStream fileStream = new(companyFactsZipFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, read, stoppingToken);

                                totalRead += read;
                                totalReads += 1;

                                if (totalReads % 2000 == 0)
                                {
                                    _logger.LogInformation("Downloading {bytes} MB from {totalBytes} MB ({Percentage:0}%)...", totalRead / 1_048_576, total / 1_048_576, totalRead * 1d / (total * 1d) * 100);
                                }
                            }
                        }
                        while (isMoreToRead);

                        _logger.LogInformation("Downloading {bytes} MB from {totalBytes} MB ({Percentage:0}%)...", totalRead / 1_048_576, total / 1_048_576, totalRead * 1d / (total * 1d) * 100);
                        _logger.LogInformation("Finished downloading.");
                    }
                }
                else
                {
                    _logger.LogInformation("Downloading is not necessary as {companyFactsLocation} is up to date.", companyFactsZipFile.FullName);
                }

                // 2. Unzip content
                var companyFactsFolder = new DirectoryInfo(_configuration["CompanyFactsFolder"]);
                if (!companyFactsFolder.Exists || (companyFactsZipFile.Exists && companyFactsZipFile.CreationTime.Date < DateTime.Today))
                {
                    if (companyFactsFolder.Exists)
                    {
                        companyFactsFolder.Delete(true);
                    }

                    _logger.LogInformation("Unzipping {companyFactsLocation} content to {companyFactsFolder} folder...", companyFactsZipFile.FullName, companyFactsFolder.FullName);

                    ZipFile.ExtractToDirectory(companyFactsZipFile.FullName, companyFactsFolder.FullName);

                    _logger.LogInformation("Finished unzipping {companyFactsLocation} content to {companyFactsFolder} folder.", companyFactsZipFile.FullName, companyFactsFolder.FullName);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(Worker)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
