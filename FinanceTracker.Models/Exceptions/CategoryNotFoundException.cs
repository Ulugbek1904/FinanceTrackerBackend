namespace FinanceTracker.Domain.Exceptions
{
    public class CategoryNotFoundException : AppException
    {
        public CategoryNotFoundException(string message)
            : base(message, 404) { }
    }
}
