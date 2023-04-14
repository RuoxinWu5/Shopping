using Data.Model;
using Data.Repository;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Order> AddOrder(Order order)
        {
            var Quantity = order.Product.Quantity;
            if (order.Quantity > Quantity)
            {
                throw new ArgumentException("Quantity not sufficient. Order creation failed.");
            }
            await _orderRepository.AddOrder(order);
            await _productRepository.ProductReduce(order.Product, order.Quantity);
            return order;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _orderRepository.GetOrderById(id);
        }

        public async Task PayOrder(int orderId)
        {
            await _orderRepository.PayOrder(orderId);
        }

        public async Task ConfirmReceipt(int orderId)
        {
            await _orderRepository.ConfirmReceipt(orderId);
        }

    }
}