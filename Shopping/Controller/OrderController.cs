using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IProductService productService, IUserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddOrder(OrderRequestModel orderRequestModel)
        {
            try
            {
                var order = new Order
                {
                    Quantity = orderRequestModel.Quantity,
                    Type = OrderState.TO_BE_PAID,
                    Product = await _productService.GetProductById(orderRequestModel.ProductId),
                    User = await _userService.GetBuyerById(orderRequestModel.BuyerId)
                };
                var result = await _orderService.AddOrder(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(exception.Message);
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
                    Type = order.Type
                };
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{orderId}/payment")]
        public async Task<ActionResult> PayOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order.Type == OrderState.TO_BE_PAID)
                {
                    await _orderService.PayOrder(orderId);
                    return Ok("Payment successful.");
                }
                else
                {
                    return BadRequest("Current order is not payable.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPut("{orderId}/receipt")]
        public async Task<ActionResult> ConfirmReceipt(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order.Type == OrderState.SHIPPED)
                {
                    await _orderService.PayOrder(orderId);
                    return Ok("Received the goods successfully.");
                }
                else
                {
                    return BadRequest("Current order is not receivable.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}