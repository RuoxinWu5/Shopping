using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public User User { get; set; } = null!;
    }
}