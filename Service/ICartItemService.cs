using Data.Model;
using Data.RequestModel;

namespace Service
{
    public interface ICartItemService
    {
        Task<CartItem> AddCartItem(AddProductToCartRequestModel cartItemRequestModel);
        Task<CartItem> GetCartItemById(int id);
        Task<IEnumerable<CartItem>> GetCartItemListByBuyerId(int buyerId);
    }
}