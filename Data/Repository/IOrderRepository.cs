using Data.Model;

namespace Data.Repository
{
    public interface IOrderRepository
    {
        Task AddOrder(Order order);
        Task<Order?> GetOrderById(int id);
        Task UpdateOrderState(int orderId, OrderState state);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
    }
}