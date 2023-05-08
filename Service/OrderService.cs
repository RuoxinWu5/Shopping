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

        public async Task UpdateOrderState(int orderId)
        {
            var order = await GetOrderById(orderId);
            switch (order.Status)
            {
                case OrderStatus.TO_BE_PAID:
                    order.Status = OrderStatus.PAID;
                    break;
                case OrderStatus.PAID:
                    order.Status = OrderStatus.SHIPPED;
                    break;
                case OrderStatus.SHIPPED:
                    order.Status = OrderStatus.RECEIVED;
                    break;
                default:
                    break;
            }
            await _orderRepository.UpdateOrder(order);
        }

        public async Task PayOrder(int orderId, int userId)
        {
            await IsOrderOwnedByUser(orderId, userId);
            var isExpectedOrderStatus = await IsExpectedOrderStatus(orderId, OrderStatus.TO_BE_PAID);
            if (!isExpectedOrderStatus)
            {
                throw new OrderStatusModificationException("Current order is not payable.");
            }
            await UpdateOrderState(orderId);
        }

        public async Task ConfirmReceipt(int orderId, int userId)
        {
            await IsOrderOwnedByUser(orderId, userId);//order
            var isExpectedOrderStatus = await IsExpectedOrderStatus(orderId, OrderStatus.SHIPPED);
            if (!isExpectedOrderStatus)
            {
                throw new OrderStatusModificationException("Current order is not receivable.");
            }
            await UpdateOrderState(orderId);
        }

        public async Task ShipOrder(int orderId, int userId)
        {
            await IsOrderOwnedByUser(orderId, userId);
            var isExpectedOrderStatus = await IsExpectedOrderStatus(orderId, OrderStatus.PAID);
            if (!isExpectedOrderStatus)
            {
                throw new OrderStatusModificationException("Current order is not shippable.");
            }
            await UpdateOrderState(orderId);
        }

        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            await _userService.ValidateIfSellerExist(sellerId);
            var orderLists = await _orderRepository.GetOrderListBySellerId(sellerId);
            return orderLists;
        }

        public async Task IsOrderOwnedByUser(int orderId, int userId)
        {
            var order = await GetOrderById(orderId);
            if (order.User.Id != userId)
            {
                throw new OrderOwnershipException("This order is not yours.");
            }
        }

        public async Task<bool> IsExpectedOrderStatus(int orderId, OrderStatus status)
        {
            var order = await GetOrderById(orderId);
            if (order.Status == status)
            {
                return true;
            }
            return false;
        }
    }
}