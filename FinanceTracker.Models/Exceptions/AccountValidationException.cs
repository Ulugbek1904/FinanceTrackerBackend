namespace FinanceTracker.Domain.Exceptions
{
    public class AccountValidationException : AppException
    {
        public AccountValidationException(string message)
            : base(message, 400) { }
    }
}
