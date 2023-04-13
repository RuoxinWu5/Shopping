using Data.Model;

namespace Data.Repository
{
    public interface IOrderRepository
    {
        Task AddOrder(Order order);
        Task<Order> GetOrderById(int id);
    }
}