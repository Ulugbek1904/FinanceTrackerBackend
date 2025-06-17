using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface ITransactionService
    {
        ValueTask<decimal> GetTotalExpensesByCategoryAsync(
            Guid? userId, int? categoryId, DateTime startDate, DateTime endDate);

        ValueTask<Transaction> CreateTransactionAsync(Transaction transaction);
        IQueryable<Transaction> RetrieveAllTransactions(Guid? userId);
        ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction);
        ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId);
        ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId);
    }
}
