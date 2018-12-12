using System;
using System.IO;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VendingApp.Infrastructure.Helpers;
using VendingApp.Infrastructure.Services;

namespace VendingApp.Console
{
    public class App
    {
        private readonly IConfigService _configService;
        private readonly ILogger _logger;
        private readonly IConsole _console;
        private readonly IInventoryService _inventoryService;

        public App(ILogger<App> logger, IConfigService configService, IConsole console, IInventoryService inventoryService)
        {
            _logger = logger;
            _configService = configService;
            _console = console;
            _inventoryService = inventoryService;
        }

        public void ShowHelp()
        {
            var data = _configService.GetAllData(null);
            _console.ForegroundColor = ConsoleColor.Green;
            _console.Write("\nTo buy a product insert coin (eg. -coin 11) and then select product (eg. -buy a1). Change will be given.");
            _console.WriteLine("\nOptions:");
            _console.Write("\n-help  Display help and commands");
            _console.Write("\n-products Shows products to buy with current prices");
            _console.Write("\n-buy <product-nr> If enough credits product will be dispatched and change given.");
            _console.Write("\n-coin <amount> Adds coins to credit");
            _console.Write("\n-cancel Returns coins");
            _console.Write("\n-currency <currency> Sets new currency");
            _console.Write("\n-info  Displays raw configuration");
            _console.Write("\n-import <path-to-config>  Import new configuration. " +
                              "Files for new config (config.json, inventory.json, and exchange_rates.json) must be placed within folder <path-to-config>");

            _console.Write("\nCoins credits: " + data.CoinsInSlot.ToString("#0.00"));
            _console.Write("\nCurrency: " + data.SelectedCurrency);
            _console.ForegroundColor = ConsoleColor.White;
            _console.WriteLine("\nType command or type c to exit...");
        }

        public void Run()
        {
            var command = Prompt.GetString("\nCommand:");
            ProcessCommand(command);
        }

        private void ProcessCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
            }
            else if (command == "-help")
            {
                ShowHelp();
            }
            else if (command == "-products")
            {
                ProductsCommand();
            }
            else if (command.Contains("-coin"))
            {
                CoinCommand(command);
            }
            else if (command.Contains("-buy"))
            {
                BuyCommand(command);
            }
            else if (command.Contains("-cancel"))
            {
                CancelCommand();
            }
            else if (command.Contains("-currency"))
            {
                CurrencyCommand(command);
            }
            else if (command == "-info")
            {
                var data = _configService.GetAllData(string.Empty);
                var serialized = JsonConvert.SerializeObject(data, Formatting.Indented);
                _console.WriteLine("\n Current config: \n" + serialized);
            }
            else if (command.Contains("-import"))
            {
                ImportCommand(command);
            }
            else if (command == "c")
            {
                Environment.Exit(0);
            }

            Run();
        }

        private void CurrencyCommand(string command)
        {
            var param = GetCommandParam(command, "-currency");
            if (string.IsNullOrEmpty(param))
            {
                var data = _configService.GetAllData(null);
                _console.Write("Currency: " + data.SelectedCurrency);
            }
            else
            {
                _configService.UpdateCurrency(param.ToUpper());
            }
        }

        private void CancelCommand()
        {
            _configService.CancelCoins();
        }

        private void ImportCommand(string command)
        {
            var folder = GetCommandParam(command, "-import");

            if (Directory.Exists(folder))
            {
                try
                {
                    var config = FileImportHelper.GetConfig(folder);
                    _console.WriteLine("\n Reading config.json ok");
                    var inventories = FileImportHelper.GetInventory(folder);
                    _console.WriteLine("\n Reading inventory.json ok");
                    var exchangeRates = FileImportHelper.GetRates(folder);
                    _console.WriteLine("\n Reading exchange_rates.json ok");
                    _configService.ImportData(inventories, config, exchangeRates);
                    _console.WriteLine("\n Saving new config to db completed");
                }
                catch (Exception ex)
                {
                    _console.WriteLine("\n Error during file importing: " + ex.Message);
                }
            }
            else
            {
                _console.WriteLine("\n Error. Folder don't exists. Please provide folder path with config files");
            }
        }

        private void BuyCommand(string command)
        {
            var productNrParam = GetCommandParam(command, "-buy");
            var change = _inventoryService.BuyProduct(productNrParam);
            _console.Write("Change given: {0}", change.ToString("#0.00"));
        }

        private void CoinCommand(string command)
        {
            var coinParam = GetCommandParam(command, "-coin");
            decimal coinAmount = 0;
            if (decimal.TryParse(coinParam, out coinAmount) || coinAmount == 0)
            {
                _configService.UpdateCoins(coinAmount);
            }
            else
            {
                _console.WriteLine("\nProblem with provided param: <coin>. It's not a number. \n");
            }
        }

        private static string GetCommandParam(string command, string param)
        {
            var result = command.Replace(param, string.Empty);
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Trim();
            }

            return result;
        }

        private void ProductsCommand()
        {
            var config = _configService.GetAllData(string.Empty);
            var sb = new StringBuilder();
            foreach (var inventoryModel in config.Inventories)
            {
                sb.AppendFormat("\n{0}-{1}, Quantity: {2}, Price: {3}", inventoryModel.ProductNr, inventoryModel.Name,
                    inventoryModel.Quantity, inventoryModel.PriceByRate);
            }

            _console.WriteLine("\nProducts to buy: \n" + sb.ToString());
            _console.WriteLine("\nCurrency: \n" + config.SelectedCurrency);
            _console.WriteLine("\nCoins in slot: \n" + config.CoinsInSlot.ToString("#0.00"));
        }
    }
}