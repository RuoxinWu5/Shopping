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
        public async Task AddOrder(Order order)
        {
            _context.Orders.Add(order);
            
            var findProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.Product.Id);
            if (findProduct != null)
            {
                findProduct.Quantity -= order.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public Task<Order> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }
    }
}