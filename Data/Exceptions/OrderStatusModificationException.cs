namespace Data.Exceptions
{
    public class OrderStatusModificationException : Exception
    {
        public OrderStatusModificationException(string message)
            : base(message)
        {
        }
    }
}