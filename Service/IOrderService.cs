using Data.Model;

namespace Service
{
    public interface IOrderService
    {
        Task<Order> AddOrder(Order order);
    }
}