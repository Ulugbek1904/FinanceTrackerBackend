namespace FinanceTracker.Domain.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException()
            : base("User not found.", 404, "https://httpstatuses.com/404") { }

        public UserNotFoundException(Exception innerException)
            : base("User not found.", 404, "https://httpstatuses.com/404", innerException) { }
    }
}
