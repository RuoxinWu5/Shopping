namespace Data.Model
{
    public enum UserType
    {
        BUYER = 0,
        SELLER = 1
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public UserType? Type { get; set; }
        public User(string name, string password, UserType type)
        {
            Name = name;
            Password = password;
            Type = type;
        }

        public User(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}