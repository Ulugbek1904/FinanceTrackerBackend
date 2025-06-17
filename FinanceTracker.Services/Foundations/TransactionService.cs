using FinanceTracker.Domain.Aggregates;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Foundations
{
    public class TransactionService : ITransactionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly TransactionAggregate aggregate;

        public TransactionService(
            IStorageBroker storageBroker,
            TransactionAggregate aggregate)
        {
            this.storageBroker = storageBroker;
            this.aggregate = aggregate;
        }

        public async ValueTask<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            this.aggregate.ValidateTransaction(transaction);
            await this.storageBroker.InsertAsync(transaction);

            return transaction;
        }

        public async ValueTask<decimal> GetTotalExpensesByCategoryAsync(Guid? userId, int? categoryId, DateTime startDate, DateTime endDate)
        {
            var transactions = await RetrieveAllTransactions(userId)
                .Where(t => t.CategoryId == categoryId
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate).ToListAsync();

            return transactions.Sum(t => t.Amount);
        }

        public async ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction)
        {
            this.aggregate.ValidateTransaction(transaction);

            var existingTransaction = await this.storageBroker.SelectAll<Transaction>()
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            if (existingTransaction == null)
                throw new TransactionNotFoundException("Transaction not found");

            existingTransaction.Description = transaction.Description;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.TransactionDate = transaction.TransactionDate;
            existingTransaction.TransactionType = transaction.TransactionType;
            existingTransaction.CategoryId = transaction.CategoryId;
            existingTransaction.AccountId = transaction.AccountId;

            var updatedTransaction = await this.storageBroker.UpdateAsync(existingTransaction);
            if (updatedTransaction == null)
                throw new TransactionNotFoundException("Transaction could not be updated");

            Console.WriteLine($"Tranzaksiya yangilandi: ID={updatedTransaction.Id}, Amount={updatedTransaction.Amount}");
            return updatedTransaction;
        }

        public async ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await this.storageBroker.
                SelectByIdAsync<Transaction>(transactionId);

            if (transaction == null)
                throw new TransactionNotFoundException("Transaction not found");

            await this.storageBroker.DeleteAsync(transaction);

            return transaction;
        }

        public IQueryable<Transaction> RetrieveAllTransactions(Guid? userId)
        {
            var transactions =  this.storageBroker.SelectAll<Transaction>()
                .Where(t => t.Account.UserId == userId)
                .Include(t => t.Account)
                .Include(t => t.Category);

            return transactions;
        }

        public async ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await this.storageBroker.SelectAll<Transaction>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");

            Console.WriteLine($"Retrieved transaction: ID={transaction.Id}, AVX={transaction.Amount}");
            return transaction;
        }
    }
}
