using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class TransactionValidationException : Xeption
    {
        public TransactionValidationException(string message) 
            : base(message) { }
    }
}
