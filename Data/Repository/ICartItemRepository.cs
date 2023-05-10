using Data.Model;

namespace Data.Repository
{
    public interface ICartItemRepository
    {
        Task AddCartItem(CartItem cartItem);
        Task<CartItem?> GetCartItemById(int id);
        Task<CartItem?> GetCartItemByProductIdAndBuyerId(int productId, int buyerId);
        Task UpdateCartItem(CartItem cartItem);
        Task<IEnumerable<CartItem>> GetCartItemListByBuyerId(int buyerId);
        Task DeleteCartItem(CartItem cartItem);
    }
}