using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task<Order> AddOrder(Order order);
        Task<Order> GetOrderById(int id);
        Task PayOrder(int orderId);
        Task ConfirmReceipt(int orderId);
        Task ShipOrder(int orderId);
        Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId);
    }
}