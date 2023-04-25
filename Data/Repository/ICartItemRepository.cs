using Data.Model;

namespace Data.Repository
{
    public interface ICartItemRepository
    {
        Task AddCartItem(CartItem cartItem);
        Task<CartItem> GetCartItemById(int id);
    }
}