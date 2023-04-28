using Data.Model;

namespace Data.ViewModel
{
    public class BuyerOrder
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public string SellerName { get; set; } = null!;
        public string BuyerName { get; set; } = null!;
        public OrderStatus Status { get; set; }
    }
}