namespace Data.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int SellerId { get; set; }//外键EFCore

        public Product(int id, string name, int quantity, int sellerId)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            SellerId = sellerId;
        }
    }
}