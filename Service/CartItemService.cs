using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Data.RequestModel;

namespace Service
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public CartItemService(ICartItemRepository cartRepository, IProductRepository productRepository, IUserRepository userRepository, IProductService productService, IUserService userService)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _productService = productService;
            _userService = userService;
        }

        public async Task<CartItem> AddCartItem(AddProductToCartRequestModel cartItemRequestModel)
        {
            var findCartItem = await _cartRepository.GetCartItemByProductIdAndBuyerId(cartItemRequestModel.ProductId, cartItemRequestModel.BuyerId);
            if (findCartItem != null)
            {
                findCartItem.Quantity += cartItemRequestModel.Quantity;
                CartItemInventoryCheck(cartItemRequestModel.Quantity, findCartItem.Product.Quantity);
                await _cartRepository.UpdateCartItem(findCartItem);
                return findCartItem;
            }
            else
            {
                var product = await _productService.GetProductById(cartItemRequestModel.ProductId);
                CartItemInventoryCheck(cartItemRequestModel.Quantity, product.Quantity);
                var cartItem = new CartItem
                {
                    Quantity = cartItemRequestModel.Quantity,
                    Product = product,
                    User = await _userService.GetBuyerById(cartItemRequestModel.BuyerId)
                };
                await _cartRepository.AddCartItem(cartItem);
                return cartItem;
            }
        }

        public async Task<CartItem> GetCartItemById(int id)
        {
            var CartItem = await _cartRepository.GetCartItemById(id);
            if (CartItem != null)
            {
                return CartItem;
            }
            throw new CartItemNotFoundException("The cart item doesn't exist.");
        }

        public async Task<IEnumerable<CartItem>> GetCartItemListByBuyerId(int buyerId)
        {
            await _userService.ValidateIfBuyerExist(buyerId);
            var cartItemList = await _cartRepository.GetCartItemListByBuyerId(buyerId);
            return cartItemList;
        }

        private void CartItemInventoryCheck(int quantity, int inventory)
        {
            if (quantity > inventory)
            {
                throw new ArgumentException("Quantity not sufficient. CartItem creation failed.");
            }
        }

        public void IsCartItemOwnedByUser(CartItem cartItem, int userId)
        {
            if (cartItem.User.Id != userId)
            {
                throw new CartItemOwnershipException("This cart item is not yours.");
            }
        }

        public async Task DeleteCartItemById(int id)
        {
            var cartItem = await GetCartItemById(id);
            await _cartRepository.DeleteCartItem(cartItem);
        }
    }
}