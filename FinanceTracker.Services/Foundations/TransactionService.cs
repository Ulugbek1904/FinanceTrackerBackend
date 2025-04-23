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

        public async ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction)
        {
            var existingTransaction = await this.storageBroker
                .SelectByIdAsync<Transaction>(transaction.Id);

            if (existingTransaction == null)
                throw new TransactionNotFoundException("Transaction not found");

            this.aggregate.ValidateTransaction(transaction);
            await this.storageBroker.UpdateAsync(transaction);

            return transaction;
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

        public IQueryable<Transaction> RetrieveAllTransactions(Guid userId)
        {
            var transactions =  this.storageBroker.SelectAll<Transaction>()
                .Where(t => t.Account.UserId == userId)
                .Include(t => t.Account)
                .Include(t => t.Category);

            return transactions;
        }

        public ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId)
        {
            return this.storageBroker.SelectByIdAsync<Transaction>(transactionId);
        }
    }
}
