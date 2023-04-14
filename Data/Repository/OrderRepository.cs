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

        public async Task<Order> GetOrderById(int id)
        {
            var result = await _context.Orders.Include(p => p.User).Include(p => p.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("The order doesn't exist.");
            }
            else
            {
                return result;
            }
        }

        public async Task PayOrder(int orderId)
        {
            var order = await GetOrderById(orderId);
            order.Type = OrderState.PAID;
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmReceipt(int orderId)
        {
            var order = await GetOrderById(orderId);
            order.Type = OrderState.RECEIVED;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            var result = await _context.Orders.Include(o => o.User).Include(o => o.Product).Where(order => order.Product.User.Id == sellerId).ToListAsync();
            return result;
        }

        public Task ShipOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}