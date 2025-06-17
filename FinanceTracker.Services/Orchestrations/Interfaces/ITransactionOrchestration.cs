using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs.PageDto;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface ITransactionOrchestration
    {
        ValueTask<IEnumerable<TransactionDto>> GetTransactionsByBudgetAsync(
                Guid userId, DateTime startDate, DateTime endDate, int categoryId);
        public ValueTask<PagedResult<TransactionDto>> RetrieveTransactionsWithQueryAsync(
                Guid userId, TransactionQueryDto queryDto);
        public ValueTask<TransactionDto> AddTransactionAsync(Guid? userId, Transaction transaction);
        public IQueryable<Transaction> RetrieveAllTransactions(Guid? userId);
        public ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId);
        public ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction);

    }
}
