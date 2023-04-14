using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required(ErrorMessage = "User name cannot be empty.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Password cannot be empty.")]
        public string Password { get; set; } = null!;
        [Range(0, 1, ErrorMessage = "Type can only be 0 AS Buyer and 1 AS Seller.")]
        public UserType? Type { get; set; }
        public List<Product>? Products { get; set; }
    }
}