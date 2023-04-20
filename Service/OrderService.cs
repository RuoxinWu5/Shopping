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
            var quantity = order.Product.Quantity;
            if (order.Quantity > quantity)
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

        public async Task UpdateOrderState(int orderId, OrderState state)
        {
            await _orderRepository.UpdateOrderState(orderId, state);
        }
        
        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            var result = await _orderRepository.GetOrderListBySellerId(sellerId);
            return result;
        }
    }
}