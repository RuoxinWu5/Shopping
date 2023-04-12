using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public User User { get; set; } = null!;
    }
}