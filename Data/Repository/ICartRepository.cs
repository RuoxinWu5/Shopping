using Data.Model;

namespace Data.Repository
{
    public interface ICartRepository
    {
        Task AddCartItem(CartItem cartItem);
        Task<CartItem> GetCartItemById(int id);
    }
}