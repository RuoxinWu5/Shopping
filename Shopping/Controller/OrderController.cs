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
                    ProductId = orderRequestModel.ProductId,
                    Quantity = orderRequestModel.Quantity,
                    BuyerId = orderRequestModel.BuyerId,
                    Type = OrderType.TO_BE_PAID,
                    Product = _productService.GetProductById(orderRequestModel.ProductId),
                    User = _userService.GetUserById(orderRequestModel.BuyerId)
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
    }
}