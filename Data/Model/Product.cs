using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    [Index(nameof(Product.Name), IsUnique = true)]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public int Quantity { get; set; }
        public int SellerId { get; set; }
        public User User { get; set; } = null!;
    }
}