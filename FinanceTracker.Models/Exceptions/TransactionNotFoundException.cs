using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class TransactionNotFoundException : Xeption
    {
        public TransactionNotFoundException() : base("Transaction not found.") { }
    }
}
