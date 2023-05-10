using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Data.RequestModel;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICartItemService _cartItemService;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository, IUserService userService, IProductService productService, ICartItemService cartItemService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _userService = userService;
            _productService = productService;
            _cartItemService = cartItemService;
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

        public async Task UpdateOrderState(Order order)
        {
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
            var order = await GetOrderById(orderId);
            IsOrderOwnedByUser(order, userId);
            var isExpectedOrderStatus = IsExpectedOrderStatus(order, OrderStatus.TO_BE_PAID);
            if (!isExpectedOrderStatus)
            {
                throw new OrderStatusModificationException("Current order is not payable.");
            }
            await UpdateOrderState(order);
        }

        public async Task ConfirmReceipt(int orderId, int userId)
        {
            var order = await GetOrderById(orderId);
            IsOrderOwnedByUser(order, userId);
            var isExpectedOrderStatus = IsExpectedOrderStatus(order, OrderStatus.SHIPPED);
            if (!isExpectedOrderStatus)
            {
                throw new OrderStatusModificationException("Current order is not receivable.");
            }
            await UpdateOrderState(order);
        }

        public async Task ShipOrder(int orderId, int userId)
        {
            var order = await GetOrderById(orderId);
            IsOrderOwnedByUser(order, userId);
            var isExpectedOrderStatus = IsExpectedOrderStatus(order, OrderStatus.PAID);
            if (!isExpectedOrderStatus)
            {
                throw new OrderStatusModificationException("Current order is not shippable.");
            }
            await UpdateOrderState(order);
        }

        public async Task<IEnumerable<Order>> GetOrderListBySellerId(int sellerId)
        {
            await _userService.ValidateIfSellerExist(sellerId);
            var orderLists = await _orderRepository.GetOrderListBySellerId(sellerId);
            return orderLists;
        }


        public void IsOrderOwnedByUser(Order order, int userId)
        {
            if (order.User.Id != userId)
            {
                throw new OrderOwnershipException("This order is not yours.");
            }
        }

        public bool IsExpectedOrderStatus(Order order, OrderStatus status)
        {
            if (order.Status == status)
            {
                return true;
            }
            return false;
        }
        public async Task<Order> AddOrderFromCartItem(AddOrderFromCartItemRequestModel addOrderFromCartItemRequestModel)
        {
            var cartItem = await _cartItemService.GetCartItemById(addOrderFromCartItemRequestModel.CartItemId);
            _cartItemService.IsCartItemOwnedByUser(cartItem,addOrderFromCartItemRequestModel.BuyerId);
            var order = new Order
            {
                Quantity = cartItem.Quantity,
                Status = OrderStatus.TO_BE_PAID,
                Product = cartItem.Product,
                User = cartItem.User
            };
            await AddOrderAndReduceProductQuantity(order);
            return order;
        }
    }
}