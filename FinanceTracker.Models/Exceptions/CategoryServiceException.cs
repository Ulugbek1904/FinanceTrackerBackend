namespace FinanceTracker.Domain.Exceptions
{
    public class CategoryServiceException : AppException
    {
        public CategoryServiceException(string message, Exception innerException)
            : base(message, 500, "https://httpstatuses.com/500", innerException)
        {
        }
    }
}
