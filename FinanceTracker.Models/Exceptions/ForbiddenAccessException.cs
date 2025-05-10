namespace FinanceTracker.Domain.Exceptions
{
    public class ForbiddenAccessException : AppException
    {
        public ForbiddenAccessException(string message = "You do not have permission to access this resource")
          : base(message, 403, "https://httpstatuses.com/400") { }

        public ForbiddenAccessException(Exception innerException)
            : base("You do not have permission to access this resource", 403, "https://httpstatuses.com/400", innerException) { }
    }
}
