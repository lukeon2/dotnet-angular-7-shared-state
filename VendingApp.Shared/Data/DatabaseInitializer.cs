using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VendingApp.Infrastructure.Helpers;
using VendingApp.Infrastructure.Services;

namespace VendingApp.Infrastructure.Data
{
    public interface IDatabaseInitializer
    {
        Task Run(string defaultFilesFolderPath);
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly VendingDbContext _context;
        private readonly ILogger _logger;
        private readonly IConfigService _configService;

        public DatabaseInitializer(VendingDbContext context, ILogger<DatabaseInitializer> logger,
            IConfigService configService)
        {
            _context = context;
            _logger = logger;
            _configService = configService;
        }

        public async Task Run(string defaultFilesFolderPath)
        {
            try
            {
                _logger.LogInformation("Starting db. Checking connection to SQL server");

                await _context.Database.MigrateAsync().ConfigureAwait(false);

                if (!await _context.Inventory.AnyAsync())
                {
                    _logger.LogInformation("Empty database. Importing default config");
                    var config = FileImportHelper.GetConfig(defaultFilesFolderPath);
                    var inventories = FileImportHelper.GetInventory(defaultFilesFolderPath);
                    var exchangeRates = FileImportHelper.GetRates(defaultFilesFolderPath);

                    _configService.ImportData(inventories, config, exchangeRates);

                    _logger.LogInformation("Default setup completed");
                }

                _configService.UpdateCoins(0, true);
                _logger.LogInformation("Db started succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Init db failed", ex);
                System.Console.WriteLine("\nInit db failed. Please check log for more details.");
                throw;
            }
        }
    }
}