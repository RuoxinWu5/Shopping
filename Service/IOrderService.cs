using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task AddOrderAndReduceProductQuantity(Order order);
        Task<Order> GetOrderById(int id);
        Task UpdateOrderState(int orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
    }
}