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

        public OrderController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddOrder(OrderRequestModel orderRequestModel)
        {
            try
            {
                var result = await _orderService.AddOrder(orderRequestModel);
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