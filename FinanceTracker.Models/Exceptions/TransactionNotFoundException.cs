namespace FinanceTracker.Domain.Exceptions
{
    public class TransactionNotFoundException : AppException
    {
        public TransactionNotFoundException(string message)
            : base(message, 404, "https://httpstatuses.com/404") { }

        public TransactionNotFoundException(string message, Exception innerException)
            : base(message, 404, "https://httpstatuses.com/404", innerException) { }
    }
}
