using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task AddOrderAndReduceProductQuantity(Order order);
        Task<Order> GetOrderById(int id);
        Task UpdateOrderState(int orderId);
        Task PayOrder(int orderId, int userId);
        Task ConfirmReceipt(int orderId, int userId);
        Task ShipOrder(int orderId, int userId);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
        Task IsOrderOwnedByUser(int orderId, int userId);
        Task<bool> IsExpectedOrderStatus(int orderId, OrderStatus status);
    }
}