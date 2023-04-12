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
    [Table("Orders")]
    public class Order
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public OrderType Type { get; set; }
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}