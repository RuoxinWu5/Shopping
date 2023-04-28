using Data.Model;

namespace Data.Repository
{
    public interface IOrderRepository
    {
        Task AddOrder(Order order);
        Task<Order?> GetOrderById(int id);
        Task UpdateOrder(Order order);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
    }
}