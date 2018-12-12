using System.ComponentModel.DataAnnotations;

namespace VendingApp.Infrastructure.Entities
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Symbol { get; set; }
    }
}