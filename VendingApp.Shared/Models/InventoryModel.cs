namespace VendingApp.Infrastructure.Models
{
    public class InventoryModel
    {
        public string ProductNr { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? TargetCurrencyRate { get; set; }

        public decimal PriceByRate => Price * TargetCurrencyRate.GetValueOrDefault(1);
    }
}