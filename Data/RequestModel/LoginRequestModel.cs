using System.ComponentModel.DataAnnotations;

namespace Data.RequestModel
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "User name cannot be empty.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Password cannot be empty.")]
        public string Password { get; set; } = null!;
    }
}