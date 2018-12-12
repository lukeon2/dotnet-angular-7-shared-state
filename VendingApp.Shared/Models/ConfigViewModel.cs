using System.Collections.Generic;

namespace VendingApp.Infrastructure.Models
{
    public class ConfigViewModel
    {
        public List<InventoryModel> Inventories { get; set; }
        public List<ExchangeRateModel> Rates { get; set; }
        public List<string> SupportedCurrencies { get; set; }

        public string Language { get; set; }

        public string InventoryCurrency { get; set; }

        public string SelectedCurrency { get; set; }

        public decimal CoinsInSlot { get; set; }
    }
}