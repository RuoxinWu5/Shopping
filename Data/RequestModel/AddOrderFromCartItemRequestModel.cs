using System.ComponentModel.DataAnnotations;

namespace Data.RequestModel
{
    public class AddOrderFromCartItemRequestModel
    {
        [Required(ErrorMessage = "Buyer Id cannot be empty.")]
        public int BuyerId { get; set; }
        [Required(ErrorMessage = "OrderItem Id cannot be empty.")]
        public int CartItemId { get; set; }
    }
}