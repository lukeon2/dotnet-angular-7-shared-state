using System.ComponentModel.DataAnnotations;

namespace VendingApp.Infrastructure.Entities
{
    public class Config
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public Currency InventoryCurrency { get; set; }

        [Required]
        public Currency SelectedCurrency { get; set; }

        public decimal CoinsInSlot { get; set; }
    }
}