using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ShoppingDbContext _context;

        public CartItemRepository(ShoppingDbContext context)
        {
            _context = context;
        }
        public async Task AddCartItem(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> GetCartItemById(int id)
        {
            var result = await _context.CartItems.Include(p => p.User).Include(p => p.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("The cartItem doesn't exist.");
            }
            else
            {
                return result;
            }
        }
    }
}