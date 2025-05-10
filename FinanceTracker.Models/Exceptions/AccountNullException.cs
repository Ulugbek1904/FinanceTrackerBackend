namespace FinanceTracker.Domain.Exceptions
{
    public class AccountNullException : AppException
    {
        public AccountNullException(string message = "Account is null.")
            : base(message, 400, "https://httpstatuses.com/400") { }

        public AccountNullException(Exception innerException)
            : base("Account not found.", 400, "https://httpstatuses.com/400", innerException) { }
    }
}
