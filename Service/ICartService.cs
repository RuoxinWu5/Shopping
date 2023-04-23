using Data.Model;

namespace Service
{
    public interface ICartService
    {
        Task<CartItem> AddCartItem(CartItem cartItem);
        Task<CartItem> GetCartItemById(int id);
        
    }
}