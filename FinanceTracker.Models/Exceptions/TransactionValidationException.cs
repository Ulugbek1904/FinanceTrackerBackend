namespace FinanceTracker.Domain.Exceptions
{
    public class TransactionValidationException : AppException
    {
        public TransactionValidationException(string message)
            : base(message, 400, "https://httpstatuses.com/400") { }

        public TransactionValidationException(string message, Exception innerException)
            : base(message, 400, "https://httpstatuses.com/400", innerException) { }
    }
}
