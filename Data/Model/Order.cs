using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public enum OrderType
    {
        TO_BE_PAID = 0,
        PAID = 1,
        SHIPPED = 2,
        RECEIVED = 3,
        CANCELLED = 4
    }
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public int BuyerId { get; set; }
        public OrderType Type { get; set; }
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}