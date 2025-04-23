using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs.PageDto;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface ITransactionOrchestration
    {
        ValueTask<PagedResult<TransactionDto>> RetrieveTransactionsWithQueryAsync(Guid userId, TransactionQueryDto queryDto);
        ValueTask<TransactionDto> AddTransactionAsync(Transaction transaction);
        public ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId);
        public IQueryable<Transaction> RetrieveAllTransactions(Guid userId);
        public ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId);
        public ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction);

    }
}
