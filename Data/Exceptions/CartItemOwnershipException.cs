namespace Data.Exceptions
{
    public class CartItemOwnershipException : Exception
    {
        public CartItemOwnershipException(string message)
            : base(message)
        {
        }
    }
}