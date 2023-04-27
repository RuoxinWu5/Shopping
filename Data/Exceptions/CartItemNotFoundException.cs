namespace Data.Exceptions
{
    public class CartItemNotFoundException : Exception
    {
        public CartItemNotFoundException(string message)
            : base(message)
        {
        }
    }
}