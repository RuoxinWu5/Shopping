using Data.Exceptions;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/seller/{sellerId}/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductListBySellerId(int sellerId)
        {
            var result = await _productService.GetProductListBySellerId(sellerId);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddProduct(Product product)
        {
            try
            {
                await _productService.AddProduct(product);
                return CreatedAtAction(nameof(AddProduct), new { id = product.Id }, "Create product successfully.");
            }
            catch (DuplicateUserNameException exception)
            {
                return Conflict(exception.Message);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}