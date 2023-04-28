using Data.Model;

namespace Data.ViewModel
{
    public class SellerOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = null!;
        public OrderStatus Status { get; set; }
    }
}