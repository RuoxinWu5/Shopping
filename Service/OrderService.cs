using Data.Exceptions;
using Data.Model;
using Data.Repository;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository, IUserService userService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task AddOrderAndReduceProductQuantity(Order order)
        {
            var product = await _productRepository.GetProductById(order.Product.Id) ?? throw new ProductNotFoundException("The product doesn't exist.");
            var inventory = product.Quantity;
            if (order.Quantity > inventory)
            {
                throw new ArgumentException("Quantity not sufficient. Order creation failed.");
            }
            await _productRepository.ReduceProductQuantity(order.Product, order.Quantity);
            await _orderRepository.AddOrder(order);
        }

        public async Task<Order> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order != null)
            {
                return order;
            }
            throw new OrderNotFoundException("The order doesn't exist.");
        }

        public async Task UpdateOrderState(int orderId, OrderState state)
        {
            await _orderRepository.UpdateOrderState(orderId, state);
        }

        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            await _userService.ValidateIfSellerExist(sellerId);
            var orderLists = await _orderRepository.GetOrderListBySellerId(sellerId);
            return orderLists;
        }
    }
}