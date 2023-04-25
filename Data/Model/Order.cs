namespace Data.Model
{
    public enum OrderState
    {
        TO_BE_PAID = 0,
        PAID = 1,
        SHIPPED = 2,
        RECEIVED = 3,
        CANCELLED = 4
    }
    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public OrderState Status { get; set; }
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}