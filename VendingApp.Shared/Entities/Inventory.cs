using System.ComponentModel.DataAnnotations;

namespace VendingApp.Infrastructure.Entities
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, 10)]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ProductNr { get; set; }
    }
}