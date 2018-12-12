using System.ComponentModel.DataAnnotations;

namespace VendingApp.Infrastructure.Entities
{
    public class ExchangeRate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Currency BaseCurrency { get; set; }

        [Required]
        public Currency TargetCurrency { get; set; }

        [Required]
        public decimal Rate { get; set; }
    }
}