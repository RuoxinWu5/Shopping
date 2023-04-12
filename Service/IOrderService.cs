using Data.Model;
using Data.ViewModel;

namespace Service
{
    public interface IOrderService
    {
        Task<Order> AddOrder(Order order);
    }
}