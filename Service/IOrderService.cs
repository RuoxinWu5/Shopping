using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task AddOrderAndReduceProductQuantity(Order order);
        Task<Order> GetOrderById(int id);
        Task UpdateOrderState(int orderId);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
        Task<bool> IsOrderOwnedByUser(int orderId, int userId);
        Task<bool> IsExpectedOrderStatus(int orderId, OrderStatus status);
    }
}