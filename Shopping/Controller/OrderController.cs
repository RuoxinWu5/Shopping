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

            var Quantity = _productService.GetProductById(orderRequestModel.ProductId).Quantity;
            if (orderRequestModel.Quantity > Quantity)
            {
                return BadRequest("Quantity not sufficient. Order creation failed.");
            }
            try
            {
                var result = await _orderService.AddOrder(orderRequestModel);
                return CreatedAtAction(nameof(AddOrder), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}