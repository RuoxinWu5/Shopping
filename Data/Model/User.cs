namespace Data.Model
{
    public enum UserType : ushort
    {
        BUYER = 0,
        SELLER = 1
    }
    public class User
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public UserType? type { get; set; }
    }
}