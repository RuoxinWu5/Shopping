using Data.Exceptions;
using Data.Model;
using Data.Repository;

namespace Service
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public CartItemService(ICartItemRepository cartRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task AddCartItem(CartItem cartItem)
        {
            var inventory = cartItem.Product.Quantity;
            var findCartItem = await _cartRepository.GetCartItemByProductIdAndBuyerId(cartItem.Product.Id, cartItem.User.Id);
            if (findCartItem == null)
            {
                CartItemInventoryCheck(cartItem, inventory);
                await _cartRepository.AddCartItem(cartItem);
            }
            else
            {
                findCartItem.Quantity += cartItem.Quantity;
                CartItemInventoryCheck(findCartItem, inventory);
                await _cartRepository.UpdateCartItem(findCartItem);
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

        private void CartItemInventoryCheck(CartItem cartItem, int inventory)
        {
            if (cartItem.Quantity > inventory)
            {
                throw new ArgumentException("Quantity not sufficient. CartItem creation failed.");
            }
        }
    }
}