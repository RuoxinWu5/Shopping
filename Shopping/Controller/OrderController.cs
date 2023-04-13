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
                    Type = OrderType.TO_BE_PAID,
                    Product = await _productService.GetProductById(orderRequestModel.ProductId),
                    User = await _userService.GetBuyerById(orderRequestModel.BuyerId)
                };
                var result = await _orderService.AddOrder(order);
                return CreatedAtAction(nameof(AddOrder), new { id = result.Id }, result);
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
                var result = new BuyerOrder
                {
                    Id = order.Id,
                    ProductName = order.Product.Name,
                    Quantity = order.Quantity,
                    SellerName = order.Product.User.Name,
                    BuyerName = order.User.Name,
                    Type = order.Type
                };
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}