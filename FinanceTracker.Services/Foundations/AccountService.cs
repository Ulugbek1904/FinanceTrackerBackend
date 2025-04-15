using FinanceTracker.Domain.Aggregates;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;

namespace FinanceTracker.Services.Foundations
{
    public class AccountService : IAccountService
    {
        private readonly IStorageBroker storageBroker;
        private readonly AccountAggregate accountAggregate = new();

        public AccountService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public IQueryable<Account> GetAccountsByUserId(Guid userId)
        {
            return this.storageBroker.SelectAll<Account>()
                .Where(account => account.UserId == userId);
        }
        public async ValueTask<Account> CreateAccountAsync(Account account)
        {
            this.accountAggregate.ValidateAccount(account);

            return await this.storageBroker.InsertAsync(account);
        }

        public async ValueTask<Account> GetAccountByIdAsync(Guid accountId)
        {

            var account = await this.storageBroker
                .SelectByIdAsync<Account>(accountId);

            if( account is null)
                throw new ArgumentNullException(nameof(accountId));

            return account;
        }

        public async ValueTask<Account> UpdateAccountBalanceAsync(Guid accountId, decimal amount)
        {
            var account = await this.storageBroker.SelectByIdAsync<Account>(accountId);
            account.Balance += amount;

            return await this.storageBroker.UpdateAsync(account);
        }

        public async ValueTask<Account> UpdateAccountAsync(Account account)
        {
            var updatedAccount = await this.storageBroker.UpdateAsync(account);

            if (updatedAccount is null)
                throw new ArgumentNullException(nameof(account));

            return updatedAccount;
        }

        public async ValueTask<Account> DeleteAccountAsync(Account account)
        {
            var deletedAccount = await this.storageBroker.DeleteAsync(account);

            if (deletedAccount is null)
                throw new ArgumentNullException(nameof(account));

            return deletedAccount;
        }
    }
}
