using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VendingApp.Infrastructure;
using VendingApp.Infrastructure.Helpers;
using VendingApp.Infrastructure.Models;
using VendingApp.Web.Controllers;
using Xunit;

namespace VendingApp.Test
{

    public class PriceHelperTest
    {
        [Fact]
        public void GetRate_WhenInventoryBaseRateSameAsSelected_ReturnsBaseRate()
        {
            var rates = GetTestRates();
            var rate = PriceHelper.GetRate(CurrencyNames.USD, CurrencyNames.USD, rates);

            Assert.True(rate == 1);
        }

        [Fact]
        public void GetRate_WhenInventoryBaseRateIsDifferentThanSelected_ReturnsRate()
        {
            var rates = GetTestRates();
            var rateUsdToEur = PriceHelper.GetRate(CurrencyNames.USD, CurrencyNames.EUR, rates);
            var rateEurToSek = PriceHelper.GetRate(CurrencyNames.EUR, CurrencyNames.SEK, rates);

            Assert.True(rateUsdToEur == (decimal)0.87);
            Assert.True(rateEurToSek == (decimal)10.30);
        }

        private static List<ExchangeRateModel> GetTestRates()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            var defaultRates = FileImportHelper.GetRates(Path.Combine(dirPath, "TestResources"));
            return defaultRates;
        }
    }
}
