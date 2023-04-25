using Data.Model;

namespace Service
{
    public interface ICartItemService
    {
        Task<CartItem> AddCartItem(CartItem cartItem);
        Task<CartItem> GetCartItemById(int id);

    }
}