using System.ComponentModel.DataAnnotations;

namespace Data.ViewModel
{
    public class AddProductRequestModel
    {
        [Required(ErrorMessage = "Product name cannot be empty.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Quantity cannot be empty.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }
        public int SellerId { get; set; }
    }
}