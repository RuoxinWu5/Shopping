using Data.Model;
using Data.RequestModel;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public CartItemsController(ICartItemService cartService, IProductService productService, IUserService userService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> AddCartItem(AddProductToCartRequestModel orderRequestModel)
        {
            try
            {
                var cartItem = new CartItem
                {
                    Quantity = orderRequestModel.Quantity,
                    Product = await _productService.GetProductById(orderRequestModel.ProductId),
                    User = await _userService.GetBuyerById(orderRequestModel.BuyerId)
                };
                var result = await _cartService.AddCartItem(cartItem);
                return CreatedAtAction(nameof(GetCartItemById), new { cartItemId = result.Id }, result);
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

        [HttpGet("{cartItemId}")]
        public async Task<ActionResult> GetCartItemById(int cartItemId)
        {
            try
            {
                var cartItem = await _cartService.GetCartItemById(cartItemId);
                var product = await _productService.GetProductById(cartItem.Product.Id);
                var buyer = await _userService.GetBuyerById(cartItem.User.Id);
                var result = new BuyerCartItem
                {
                    ProductName = product.Name,
                    Quantity = cartItem.Quantity,
                    BuyerName = buyer.Name,
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