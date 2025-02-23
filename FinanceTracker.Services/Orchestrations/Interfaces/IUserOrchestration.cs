using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface IUserOrchestration
    {
        ValueTask<User> RegisterUserAsync(User user);
    }
}
