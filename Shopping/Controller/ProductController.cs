using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{sellerId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductListBySellerId(int sellerId)
        {
            var result = await _productService.GetProductListBySellerId(sellerId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}