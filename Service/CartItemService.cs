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
        public async Task<CartItem> AddCartItem(CartItem cartItem)
        {
            var quantity = cartItem.Product.Quantity;
            if (cartItem.Quantity > quantity)
            {
                throw new ArgumentException("Quantity not sufficient. CartItem creation failed.");
            }
            await _cartRepository.AddCartItem(cartItem);
            return cartItem;
        }

        public async Task<CartItem> GetCartItemById(int id)
        {
            return await _cartRepository.GetCartItemById(id);
        }
    }
}