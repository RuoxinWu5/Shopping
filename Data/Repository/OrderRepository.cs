using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShoppingDbContext _context;

        public OrderRepository(ShoppingDbContext context)
        {
            _context = context;
        }
        public async Task<Product> AddOrder(Order order)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(u => u.Id == order.ProductId);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"The product doesn't exists.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == order.BuyerId);
            if (existingUser == null || existingUser.Type == UserType.SELLER)
            {
                throw new KeyNotFoundException("The buyer doesn't exist.");
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return existingProduct;
        }
    }
}