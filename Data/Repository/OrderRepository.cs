using Data.Exceptions;
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
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            var result = await _context.Orders.Include(p => p.User).Include(p => p.Product).FirstOrDefaultAsync(o => o.Id == id);
            return result;
        }

        public async Task UpdateOrderState(int orderId, OrderState state)
        {
            var order = await GetOrderById(orderId) ?? throw new OrderNotFoundException("The order doesn't exist.");
            order.Status = state;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            var result = await _context.Orders.Include(o => o.User).Include(o => o.Product).Where(order => order.Product.User.Id == sellerId).ToListAsync();
            return result;
        }
    }
}