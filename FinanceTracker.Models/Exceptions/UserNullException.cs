using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class UserNullException : Xeption
    {
        public UserNullException() : base("User is null.") { }
    }
}
