namespace Data.Exceptions
{
    public class OrderOwnershipException : Exception
    {
        public OrderOwnershipException(string message)
            : base(message)
        {
        }
    }
}