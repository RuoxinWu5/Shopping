using Data.Model;

namespace Data.Repository
{
    public interface IOrderRepository
    {
        Task<Product> AddOrder(Order order);
    }
}