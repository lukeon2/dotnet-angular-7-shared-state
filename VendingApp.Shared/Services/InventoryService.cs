using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Logging;
using VendingApp.Infrastructure.Data;
using VendingApp.Infrastructure.Entities;
using VendingApp.Infrastructure.Models;

namespace VendingApp.Infrastructure.Services
{
    public interface IInventoryService
    {
        decimal BuyProduct(string inventoryId);
    }

    public class InventoryService : IInventoryService
    {
        private ILogger _logger;
        private readonly VendingDbContext _context;
        private readonly IConfigService _configService;

        public InventoryService(ILogger<InventoryService> logger, VendingDbContext context, IConfigService configService)
        {
            _logger = logger;
            _context = context;
            _configService = configService;
        }

        public decimal BuyProduct(string productNr)
        {
            productNr = productNr.ToUpperInvariant();
            var config = _configService.GetAllData(null);
            var inventoryWithPrice = config.Inventories.FirstOrDefault(x => x.ProductNr == productNr);

            Validate(inventoryWithPrice, config);
            var change = (decimal)0;
            if (config.CoinsInSlot >= inventoryWithPrice.PriceByRate)
            {
                _logger.LogInformation(string.Format("Dispatching product: {0}-{1}", inventoryWithPrice.ProductNr, inventoryWithPrice.Name));
                var inventory = _context.Inventory.First(x => x.ProductNr == productNr);
                inventory.Quantity--;
                change = config.CoinsInSlot - inventoryWithPrice.PriceByRate;
                _configService.UpdateCoins(0, true);
                _context.SaveChanges();
                _logger.LogInformation(string.Format("Product dispatched: {0}-{1} ", inventoryWithPrice.ProductNr, inventoryWithPrice.Name));
            }
            
            return change;
        }

        private void Validate(InventoryModel inventoryWithPrice, ConfigViewModel config)
        {
            if (inventoryWithPrice == null)
            {
                throw new ValidationException("No such product");
            }
            if (config.CoinsInSlot < inventoryWithPrice.PriceByRate)
            {
                throw new ValidationException("Insert coin");
            }
            if (inventoryWithPrice.Quantity == 0)
            {
                throw new ValidationException("Product amount in machine is 0");
            }
        }
    }
}