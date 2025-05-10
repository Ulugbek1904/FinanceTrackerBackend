namespace FinanceTracker.Domain.Exceptions
{
    public class UserNullException : AppException
    {
        public UserNullException()
            : base("User is null.", 400, "https://httpstatuses.com/400") { }

        public UserNullException(Exception innerException)
            : base("User is null.", 400, "https://httpstatuses.com/400", innerException) { }
    }
}
