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
        private readonly IProductService _productService;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository, IUserService userService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _userService = userService;
            _productService = productService;
        }

        public async Task AddOrderAndReduceProductQuantity(Order order)
        {
            await _productService.ReduceProductQuantity(order.Product, order.Quantity);
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

        public async Task UpdateOrderState(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetOrderById(orderId) ?? throw new OrderNotFoundException("The order doesn't exist.");
            order.Status = status;
            await _orderRepository.UpdateOrder(order);
        }

        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            await _userService.ValidateIfSellerExist(sellerId);
            var orderLists = await _orderRepository.GetOrderListBySellerId(sellerId);
            return orderLists;
        }

        public async Task<bool> IsOrderOwnedByUser(int orderId, int userId)
        {
            var order = await GetOrderById(orderId);
            if (order.User.Id == userId){
                return true;
            }
            return false;
        }
    }
}