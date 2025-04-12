using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Orchestrations
{
    public class TransactionOrchestration : ITransactionOrchestration
    {
        private readonly ICategoryService categoryService;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public TransactionOrchestration(
            ICategoryService categoryService,
            IAccountService accountService,
            ITransactionService transactionService)
        {
            this.categoryService = categoryService;
            this.accountService = accountService;
            this.transactionService = transactionService;
        }

        public async ValueTask<Transaction> AddTransactionAsync(Transaction transaction)
        {
            var existingCategory = this.categoryService
                .RetrieveAllCategories()
                .FirstOrDefault(c => c.Id == transaction.CategoryId);

            if (existingCategory is null)
                throw new InvalidOperationException("Category not found.");

            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account is null)
                throw new InvalidOperationException("Account not found.");

            if (transaction.TransactionType == TransactionType.Expense)
            {
                if (account.Balance < transaction.Amount)
                    throw new InvalidOperationException("Insufficient funds.");
                account.Balance -= transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                account.Balance += transaction.Amount;
            }

            await this.accountService.UpdateAccountBalanceAsync(account.Id, account.Balance);

            return await transactionService.CreateTransactionAsync(transaction);
        }

        public async ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction)
        {
            var existingTransaction = await this.transactionService.RetrieveTransactionByIdAsync(transaction.Id);
            if (existingTransaction == null)
                throw new InvalidOperationException("Transaction not found.");

            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            if (existingTransaction.TransactionType == TransactionType.Expense)
            {
                account.Balance += existingTransaction.Amount; 
            }
            else if (existingTransaction.TransactionType == TransactionType.Income)
            {
                account.Balance -= existingTransaction.Amount; 
            }

            if (transaction.TransactionType == TransactionType.Expense)
            {
                if (account.Balance < transaction.Amount)
                    throw new InvalidOperationException("Insufficient funds.");
                account.Balance -= transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                account.Balance += transaction.Amount;
            }

            await this.accountService.UpdateAccountBalanceAsync(account.Id, account.Balance);
            return await transactionService.ModifyTransactionAsync(transaction);
        }

        public async ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await this.transactionService.RetrieveTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found.");

            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            if (transaction.TransactionType == TransactionType.Expense)
            {
                account.Balance += transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                account.Balance -= transaction.Amount;
            }

            await this.accountService.UpdateAccountBalanceAsync(account.Id, account.Balance);
            return await transactionService.RemoveTransactionByIdAsync(transactionId);
        }

        public IQueryable<Transaction> RetrieveAllTransactions(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var transactions = transactionService.RetrieveAllTransactions(userId);

            if (!transactions.Any())
                throw new TransactionNotFoundException();

            return transactions;
        }

        public async ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
                throw new ArgumentException("Invalid transaction ID.", nameof(transactionId));

            var transaction = await transactionService.RetrieveTransactionByIdAsync(transactionId);

            if (transaction is null)
                throw new TransactionNotFoundException();

            return transaction;
        }

    }

}
