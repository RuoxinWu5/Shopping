using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task<Order> AddOrder(Order order);
        Task<Order> GetOrderById(int id);
        Task UpdateOrderState(int orderId, OrderState state);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
    }
}