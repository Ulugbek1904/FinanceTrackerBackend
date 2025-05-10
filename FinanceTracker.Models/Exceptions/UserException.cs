namespace FinanceTracker.Domain.Exceptions
{
    public class UserException : AppException
    {
        public UserException(string message)
            : base(message, 400, "https://httpstatuses.com/500") { }

        public UserException(string message, Exception innerException)
            : base(message, 400, "https://httpstatuses.com/500", innerException) { }
    }
}
