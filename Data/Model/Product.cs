using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Product name cannot be empty.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Quantity cannot be empty.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }
        public int SellerId { get; set; }
        public User User { get; set; } = null!;
    }
}