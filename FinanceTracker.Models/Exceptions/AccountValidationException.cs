using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class AccountValidationException : Xeption
    {
        public AccountValidationException(string message)
                       : base(message) { }
    }
}
