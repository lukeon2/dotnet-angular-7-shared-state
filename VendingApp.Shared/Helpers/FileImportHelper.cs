using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VendingApp.Infrastructure.Models;

namespace VendingApp.Infrastructure.Helpers
{
    public static class FileImportHelper
    {
        public static string GenerateProductNr(int productIndex)
        {
            var rowsTemplate = "ABCDE";
            var rowResult = string.Empty;
            var columnResult = string.Empty;

            var index = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    if (index == productIndex)
                    {
                        rowResult = rowsTemplate[i].ToString();
                        columnResult = j.ToString();
                        return string.Format("{0}{1}", rowResult, columnResult);
                    }

                    index++;
                }
            }

            return string.Empty;
        }

        private static int GetRowNr(int index)
        {
            if (index == 10)
            {
                return 0;
            }

            var rowNr = index / 10;
            if (rowNr > 1)
            {
                rowNr = rowNr - 1;
            }

            return rowNr;
        }

        public static List<ExchangeRateModel> GetRates(string folderPath)
        {
            var configJson = System.IO.File.ReadAllText(Path.Combine(folderPath, "exchange_rates.json"));
            var aaa = JArray.Parse(configJson);
            var exchangeRatesModel = new List<ExchangeRateModel>();
            foreach (var item in aaa.Children())
            {
                var mainCurrency = item["currency"];
                foreach (var rate in item["exchange_rates"].Children())
                {
                    var jProperty = rate.First.ToObject<JProperty>();
                    exchangeRatesModel.Add(new ExchangeRateModel()
                    {
                        BaseCurrency = mainCurrency.ToString(),
                        TargetCurrency = jProperty.Name,
                        Rate = decimal.Parse(jProperty.Value.ToString())
                    });
                }
            }

            return exchangeRatesModel;
        }

        public static ConfigModel GetConfig(string folderPath)
        {
            var configJson = System.IO.File.ReadAllText(Path.Combine(folderPath, "config.json"));
            return JsonConvert.DeserializeObject<ConfigModel>(configJson, new JsonSerializerSettings()
            {
                ContractResolver = new UnderscorePropertyNamesContractResolver()
            });
        }

        public static IList<InventoryModel> GetInventory(string folderPath)
        {
            var configJson = System.IO.File.ReadAllText(Path.Combine(folderPath, "inventory.json"));
            return JsonConvert.DeserializeObject<List<InventoryModel>>(configJson);
        }
    }
}
