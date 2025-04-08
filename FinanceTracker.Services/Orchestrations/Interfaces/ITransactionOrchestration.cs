using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface ITransactionOrchestration
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);
        public ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId);
        public IQueryable<Transaction> RetrieveAllTransactions(Guid userId);
        public ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId);
        public ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction);

    }
}
