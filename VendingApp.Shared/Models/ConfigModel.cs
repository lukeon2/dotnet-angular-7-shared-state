using System.Collections.Generic;

namespace VendingApp.Infrastructure.Models
{
    public class ConfigModel
    {
        public string Language { get; set; }

        public string InventoryCurrency { get; set; }

        public List<string> SupportedCurrencies { get; set; }
    }
}