using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class TransactionNotFoundException : Xeption
    {
        public TransactionNotFoundException(string message) : base(message) { }
        public TransactionNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
    }
}
