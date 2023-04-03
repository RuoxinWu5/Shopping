using System.Net;
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
            if (string.IsNullOrWhiteSpace(product.name))
            {
                return BadRequest("Product name cannot be empty.");
            }
            if (product.quantity < 0)
            {
                return BadRequest("Quantity cannot be less than zero.");
            }
            var result = await _productService.AddProduct(product);
            if (result.StatusCode == HttpStatusCode.Created)
            {
                return CreatedAtAction(nameof(AddProduct), new { id = product.id }, "Create product successfully.");
            }
            else if (result.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(await result.Content.ReadAsStringAsync());
            }
            return NotFound(await result.Content.ReadAsStringAsync());
        }
    }
}