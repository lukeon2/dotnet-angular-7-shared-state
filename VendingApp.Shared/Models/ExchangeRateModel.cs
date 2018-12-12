namespace VendingApp.Infrastructure.Models
{
    public class ExchangeRateModel
    {
        public string BaseCurrency { get; set; }

        public string TargetCurrency { get; set; }

        public decimal Rate { get; set; }
    }
}