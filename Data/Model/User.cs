using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    public enum UserType
    {
        BUYER = 0,
        SELLER = 1
    }

    [Index(nameof(User.Name), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public UserType? Type { get; set; }
        public List<Product>? Products { get; set; }
    }
}