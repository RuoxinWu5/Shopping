using Data.Exceptions;
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
        public async Task<ActionResult> AddCartItem(AddProductToCartRequestModel cartItemRequestModel)
        {
            try
            {
                var cartItem = await _cartService.AddCartItem(cartItemRequestModel);
                return CreatedAtAction(nameof(GetCartItemById), new { cartItemId = cartItem.Id }, cartItem);
            }
            catch (ProductNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (BuyerNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("{cartItemId}")]
        public async Task<ActionResult> GetCartItemById(int cartItemId)
        {
            try
            {
                var cartItem = await _cartService.GetCartItemById(cartItemId);
                return Ok(cartItem);
            }
            catch (CartItemNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuyerCartItem>>> GetCartItemListByBuyerId(int buyerId)
        {
            try
            {
                var cartItems = await _cartService.GetCartItemListByBuyerId(buyerId);
                List<BuyerCartItem> buyerCartItems = new List<BuyerCartItem>();
                foreach (CartItem cartItem in cartItems)
                {
                    var buyerCartItem = new BuyerCartItem
                    {
                        ProductName=cartItem.Product.Name,
                        Quantity=cartItem.Quantity
                    };
                    buyerCartItems.Add(buyerCartItem);
                }
                return Ok(buyerCartItems);
            }
            catch (BuyerNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}