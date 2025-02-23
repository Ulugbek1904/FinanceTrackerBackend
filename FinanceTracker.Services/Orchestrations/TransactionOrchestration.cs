using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;

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
            .FirstOrDefault(c => c.Id == transaction.Category.Id);

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

    }
}
