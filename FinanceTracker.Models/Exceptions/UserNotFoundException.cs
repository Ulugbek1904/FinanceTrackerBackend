using Xeptions;

namespace FinanceTracker.Domain.Exceptions
{
    public class UserNotFoundException : Xeption
    {
        public UserNotFoundException() : base("User not found.") { }
    }
}
