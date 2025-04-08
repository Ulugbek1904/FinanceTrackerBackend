namespace FinanceTracker.Domain.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException(string message) : base(message)
        {
        }

        public CategoryNotFoundException(string message, Exception ex) : base(message, ex)
        {
            
        }
    }
}
