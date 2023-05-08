using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task AddOrderAndReduceProductQuantity(Order order);
        Task<Order> GetOrderById(int id);
        Task UpdateOrderState(Order order);
        Task PayOrder(int orderId, int userId);
        Task ConfirmReceipt(int orderId, int userId);
        Task ShipOrder(int orderId, int userId);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
        void IsOrderOwnedByUser(Order order, int userId);
        bool IsExpectedOrderStatus(Order order, OrderStatus status);
    }
}