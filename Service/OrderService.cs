using Data.Model;
using Data.Repository;
using Data.ViewModel;

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

        public async Task<Order> AddOrder(OrderRequestModel orderRequestModel)
        {
            var order = new Order
            {
                ProductId = orderRequestModel.ProductId,
                Quantity = orderRequestModel.Quantity,
                BuyerId = orderRequestModel.BuyerId,
                Type = OrderType.TO_BE_PAID,
                Product = _productRepository.GetProductById(orderRequestModel.ProductId),
                User = _userRepository.GetUserById(orderRequestModel.BuyerId)
            };
            await _orderRepository.AddOrder(order);
            return order;
        }
    }
}