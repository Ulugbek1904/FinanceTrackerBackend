namespace FinanceTracker.Domain.Exceptions
{
    public class CategoryServiceException : Exception
    {
        public CategoryServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
