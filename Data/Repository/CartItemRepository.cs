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

        public async Task<CartItem?> GetCartItemById(int id)
        {
            var cartItem = await _context.CartItems.Include(p => p.User).Include(p => p.Product).FirstOrDefaultAsync(o => o.Id == id);
            return cartItem;
        }

        public async Task<CartItem?> GetCartItemByProductIdAndBuyerId(int productId, int buyerId)
        {
            var cartItem = await _context.CartItems.Include(p => p.User).Include(p => p.Product).FirstOrDefaultAsync(o => o.Product.Id == productId && o.User.Id == buyerId);
            return cartItem;
        }

        public async Task UpdateCartItem(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItem>> GetCartItemListByBuyerId(int buyerId)
        {
            var cartItemList = await _context.CartItems.Include(c => c.User).Include(c => c.Product).Where(cartItem => cartItem.User.Id == buyerId).ToListAsync();
            return cartItemList;
        }

        public async Task DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }
}