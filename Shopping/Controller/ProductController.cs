using Data.Exceptions;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductsController(IProductService productService, IUserService userService)
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

        [HttpPost]
        public async Task<ActionResult> AddProduct(AddProductRequestModel productRequestModel)
        {
            try
            {
                var product = new Product
                {
                    Name = productRequestModel.Name,
                    Quantity = productRequestModel.Quantity,
                    User = await _userService.GetSellerById(productRequestModel.SellerId)
                };
                var result = await _productService.AddProduct(product);
                return CreatedAtAction(nameof(GetProductById), new { productId = result.Id }, result);
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

        [HttpGet("{productId}")]
        public async Task<ActionResult> GetProductById(int productId)
        {
            try
            {
                var result = await _productService.GetProductById(productId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}