namespace Data.Model
{
    public class BuyerProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public string SellerName { get; set; } = null!;
    }
}