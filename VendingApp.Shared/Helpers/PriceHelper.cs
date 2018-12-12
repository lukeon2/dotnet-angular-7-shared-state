using System.Collections.Generic;
using System.Linq;
using VendingApp.Infrastructure.Models;

namespace VendingApp.Infrastructure.Helpers
{
    public static class PriceHelper
    {
        public static decimal GetRate(string baseCurrency, string selectedCurrency, List<ExchangeRateModel> configRates)
        {
            if (baseCurrency == selectedCurrency)
            {
                return 1;
            }

            var rate = configRates.First(x => x.BaseCurrency == baseCurrency && x.TargetCurrency == selectedCurrency);
            return rate.Rate;
        }
    }
}