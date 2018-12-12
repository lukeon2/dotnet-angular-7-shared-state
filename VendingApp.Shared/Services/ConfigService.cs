using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VendingApp.Infrastructure.Data;
using VendingApp.Infrastructure.Entities;
using VendingApp.Infrastructure.Helpers;
using VendingApp.Infrastructure.Models;

namespace VendingApp.Infrastructure.Services
{
    public interface IConfigService
    {
        void ImportData(IList<InventoryModel> inventories, ConfigModel config, List<ExchangeRateModel> exchangeRates);

        ConfigViewModel GetAllData(string currency);

        void UpdateCoins(decimal amount, bool forceValue = false);

        void UpdateCurrency(string symbol);

        decimal CancelCoins();
    }

    public class ConfigService : IConfigService
    {
        private ILogger _logger;
        private readonly VendingDbContext _context;

        public ConfigService(ILogger<ConfigService> logger, VendingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void ImportData(IList<InventoryModel> inventories, ConfigModel configModel, List<ExchangeRateModel> exchangeRates)
        {
            _logger.LogInformation("Import. Validating config files.");
            ValidateRates(exchangeRates);
            _logger.LogInformation("Validation complete. Removing old data.");
            _context.ExchangeRate.RemoveRange(_context.ExchangeRate);
            _context.Config.RemoveRange(_context.Config);
            _context.Currency.RemoveRange(_context.Currency);
            _context.Inventory.RemoveRange(_context.Inventory);

            _logger.LogInformation("Adding inventories and config");
            // TODO use Mapper
            AddInventories(inventories);
            
            var config = new Config()
            {
                Language = configModel.Language
            };

            _context.Config.Add(config);

            foreach (var currency in configModel.SupportedCurrencies)
            {
                var currencyEntity = new Currency()
                {
                    Symbol = currency
                };

                if (currencyEntity.Symbol == configModel.InventoryCurrency)
                {
                    config.InventoryCurrency = currencyEntity;
                    config.SelectedCurrency = currencyEntity;
                }

                _context.Currency.Add(currencyEntity);
            }

            _context.SaveChanges();
            AddRates(exchangeRates);

            _logger.LogInformation("Import complete");
        }
        
        public ConfigViewModel GetAllData(string currency)
        {
            var config = _context.Config
                .Include(x => x.SelectedCurrency)
                .Include(x => x.InventoryCurrency).First();
            var currentRateSymbol = config.InventoryCurrency.Symbol;

            if (string.IsNullOrEmpty(currency))
            {
                currency = config.SelectedCurrency.Symbol;
            }

            var rates = _context.ExchangeRate.Select(x => new ExchangeRateModel()
            {
                TargetCurrency = x.TargetCurrency.Symbol,
                BaseCurrency = x.BaseCurrency.Symbol,
                Rate = x.Rate
            }).ToList();

            var rate = PriceHelper.GetRate(config.InventoryCurrency.Symbol, currency, rates);

            return new ConfigViewModel()
            {
                CoinsInSlot = config.CoinsInSlot,
                Language = config.Language,
                InventoryCurrency = currentRateSymbol,
                SelectedCurrency = config.SelectedCurrency.Symbol,
                SupportedCurrencies = _context.Currency.Select(x => x.Symbol).ToList(),

                // TODO fix string ordering
                Inventories = _context.Inventory.OrderBy(x => x.ProductNr).Select(x => new InventoryModel()
                {
                    Quantity = x.Quantity,
                    Name = x.Name,
                    Price = x.Price,
                    ProductNr = x.ProductNr,
                    TargetCurrencyRate = rate
                }).ToList(),
                Rates = rates
            };
        }

        public void UpdateCoins(decimal amount, bool forceValue = false)
        {
            if (amount < 0)
            {
                return;
            }

            var entity = _context.Config.First();
            if (forceValue)
            {
                entity.CoinsInSlot = amount;
            }
            else
            {
                entity.CoinsInSlot += amount;
            }
            
            _context.SaveChanges();
            _logger.LogInformation("Coins in slot: " + entity.CoinsInSlot.ToString("#0.00"));
        }

        public decimal CancelCoins()
        {
            var entity = _context.Config.First();
            var changeGiven = entity.CoinsInSlot;
            entity.CoinsInSlot = 0;
            _context.SaveChanges();
            _logger.LogInformation("Change reutrned: " + changeGiven.ToString("#0.00"));
            _logger.LogInformation("Coins in slot: " + entity.CoinsInSlot.ToString("#0.00"));
            return changeGiven;
        }

        public void UpdateCurrency(string symbol)
        {
            symbol = symbol.ToUpperInvariant();
            ValidateCurrency(symbol);

            var currency = _context.Currency.First(x => x.Symbol == symbol);
            var entity = _context.Config.First();
            entity.SelectedCurrency = currency;
            _context.SaveChanges();
            _logger.LogInformation("Currency set to: " + symbol);
        }
        
        private void AddRates(IList<ExchangeRateModel> rates)
        {
            foreach (var rateModel in rates)
            {
                var baseCurrency = _context.Currency.First(x => x.Symbol == rateModel.BaseCurrency);
                var targetCurrency = _context.Currency.First(x => x.Symbol == rateModel.TargetCurrency);

                var rate = new ExchangeRate()
                {
                    BaseCurrency = baseCurrency,
                    TargetCurrency = targetCurrency,
                    Rate = rateModel.Rate
                };

                _context.ExchangeRate.Add(rate);
            }

            _context.SaveChanges();
        }

        private void AddInventories(IList<InventoryModel> inventories)
        {
            for (var index = 0; index < inventories.Count; index++)
            {
                var inventoryDto = inventories[index];
                var productNr = FileImportHelper.GenerateProductNr(index);
                var inventory = new Inventory()
                {
                    Name = inventoryDto.Name,
                    Price = inventoryDto.Price,
                    Quantity = inventoryDto.Quantity,
                    ProductNr = productNr
                };
                
                _context.Inventory.Add(inventory);
                _logger.LogInformation(string.Format("Importing roduct {0}", productNr));
            }
        }

        private void ValidateRates(List<ExchangeRateModel> exchangeRates)
        {
            if (exchangeRates.Any(x => x.BaseCurrency == x.TargetCurrency))
            {
                throw new ValidationException("Exchange rate import error. Currency pair with own reference");
            }
        }

        private void ValidateCurrency(string symbol)
        {
            if (_context.Config.First().CoinsInSlot > 0)
            {
                throw new ValidationException("Can't change currency when there are coins in slot");
            }

            if (!_context.Currency.Any(x => x.Symbol == symbol))
            {
                throw new ValidationException("No currency with this symbol");
            }
        }
    }
}