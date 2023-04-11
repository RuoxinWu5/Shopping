using Data.Exceptions;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/seller/{sellerId}/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductController(IProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductListBySellerId(int sellerId)
        {
            var result = await _productService.GetProductListBySellerId(sellerId);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddProduct(ProductRequestModel productViewModel)
        {
            var product = new Product
            {
                Name = productViewModel.Name,
                Quantity = productViewModel.Quantity,
                SellerId = productViewModel.SellerId,
                User = _userService.GetUserById(productViewModel.SellerId)
            };
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