using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface ITransactionService
    {
        ValueTask<Transaction> CreateTransactionAsync(Transaction transaction);
        IQueryable<Transaction> RetrieveAllTransactions(Guid userId);
        ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction);
        ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId);
    }
}
