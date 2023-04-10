using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/products")]
    [ApiController]
    public class BuyerController : ControllerBase
    {
        private readonly IBuyerService _buyerService;
        public BuyerController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        [HttpGet]
        public async Task<ActionResult> AllProduct()
        {
            var result = await _buyerService.AllProduct();
            return Ok(result);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult> GetProductByProductId(int productId)
        {
            try
            {
                var result = await _buyerService.GetProductByProductId(productId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}