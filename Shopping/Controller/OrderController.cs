using Data.Exceptions;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security.Claims;

namespace Shopping.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public OrdersController(IOrderService orderService, IProductService productService, IUserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }

        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UserNotFoundException("You are not authorized to perform this action.");
            }
            return int.Parse(userId);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrder(AddOrderRequestModel orderRequestModel)
        {
            try
            {
                var order = new Order
                {
                    Quantity = orderRequestModel.Quantity,
                    Status = OrderStatus.TO_BE_PAID,
                    Product = await _productService.GetProductById(orderRequestModel.ProductId),
                    User = await _userService.GetBuyerById(orderRequestModel.BuyerId)
                };
                await _orderService.AddOrderAndReduceProductQuantity(order);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
            }
            catch (ProductNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (BuyerNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                var product = await _productService.GetProductById(order.Product.Id);
                var buyer = await _userService.GetBuyerById(order.User.Id);
                var result = new BuyerOrder
                {
                    Id = order.Id,
                    ProductName = product.Name,
                    Quantity = order.Quantity,
                    SellerName = product.User.Name,
                    BuyerName = buyer.Name,
                    Status = order.Status
                };
                return Ok(result);
            }
            catch (OrderNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("BuyerPolicy")]
        [HttpPost("{orderId}/payment")]
        public async Task<ActionResult> PayOrder(int orderId)
        {
            try
            {
                var userId = GetUserId();
                await _orderService.PayOrder(orderId, userId);
                return Ok("Payment successful.");
            }
            catch (UserNotFoundException)
            {
                return BadRequest("You are not authorized to pay for this order.");
            }
            catch (OrderNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OrderOwnershipException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OrderStatusModificationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("BuyerPolicy")]
        [HttpPost("{orderId}/receipt")]
        public async Task<ActionResult> ConfirmReceipt(int orderId)
        {
            try
            {
                var userId = GetUserId();
                await _orderService.ConfirmReceipt(orderId, userId);
                return Ok("Received the goods successfully.");
            }
            catch (UserNotFoundException)
            {
                return BadRequest("You are not authorized to confirm the receipt of this order.");
            }
            catch (OrderNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OrderOwnershipException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OrderStatusModificationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("SellerPolicy")]
        [HttpPost("{orderId}/ship")]
        public async Task<ActionResult> ShipOrder(int orderId)
        {
            try
            {
                var userId = GetUserId();
                await _orderService.ShipOrder(orderId, userId);
                return Ok("Delivery successful.");
            }
            catch (UserNotFoundException)
            {
                return BadRequest("You are not authorized to ship this order.");
            }
            catch (OrderNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OrderOwnershipException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OrderStatusModificationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellerOrder>>> GetOrderListBySellerId(int sellerId)
        {
            try
            {
                var orders = await _orderService.GetOrderListBySellerId(sellerId);
                List<SellerOrder> result = new List<SellerOrder>();
                foreach (Order order in orders)
                {
                    var product = await _productService.GetProductById(order.Product.Id);
                    var buyer = await _userService.GetBuyerById(order.User.Id);
                    var buyerOrder = new SellerOrder
                    {
                        Id = order.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = order.Quantity,
                        BuyerId = buyer.Id,
                        BuyerName = buyer.Name,
                        Status = order.Status
                    };
                    result.Add(buyerOrder);
                }
                return Ok(result);
            }
            catch (SellerNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}