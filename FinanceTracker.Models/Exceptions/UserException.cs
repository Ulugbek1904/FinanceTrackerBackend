using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class UserException : Xeption
    {
        public UserException(string message)
            : base(message) { }
    }
}
