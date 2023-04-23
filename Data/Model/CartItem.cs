namespace Data.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public Product Product{ get; set; } = null!;
        public int Quantity { get; set; }
        public User User { get; set; } = null!;
    }
}