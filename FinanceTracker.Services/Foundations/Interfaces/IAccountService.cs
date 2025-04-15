using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IAccountService
    {
        IQueryable<Account> GetAccountsByUserId(Guid userId);
        ValueTask<Account> CreateAccountAsync(Account account);
        ValueTask<Account> UpdateAccountBalanceAsync(Guid accountId, decimal amount);
        ValueTask<Account> GetAccountByIdAsync(Guid accountId);
        ValueTask<Account> UpdateAccountAsync(Account account);
        ValueTask<Account> DeleteAccountAsync(Account account);
    }
}
