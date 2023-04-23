using System.ComponentModel.DataAnnotations;

namespace Data.RequestModel
{
    public class AddProductToCartRequestModel
    {
        [Required(ErrorMessage = "Product Id cannot be empty.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Quantity cannot be empty.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Buyer Id cannot be empty.")]
        public int BuyerId { get; set; }
    }
}