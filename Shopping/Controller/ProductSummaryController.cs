using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/products-summary")]
    [ApiController]
    public class ProductSummaryController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductSummaryController(IProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> AllProduct()
        {
            var result = await _productService.AllProduct();
            var nameList = result.Select(p => p.Name);
            return Ok(nameList);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult> GetProductByProductId(int productId)
        {
            try
            {
                var product = await _productService.GetProductById(productId);
                var result = new BuyerProduct
                {
                    Id = productId,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    SellerName = product.User.Name
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