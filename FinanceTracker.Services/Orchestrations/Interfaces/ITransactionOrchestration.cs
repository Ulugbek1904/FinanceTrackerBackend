using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface ITransactionOrchestration
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);

    }
}
