using Data.Model;

namespace Service
{
    public interface ICartItemService
    {
        Task AddCartItem(CartItem cartItem);
        Task<CartItem> GetCartItemById(int id);
    }
}