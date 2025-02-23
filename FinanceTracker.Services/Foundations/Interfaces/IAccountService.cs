using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IAccountService
    {
        ValueTask<Account> CreateAccountAsync(Account account);
        ValueTask<Account> UpdateAccountBalanceAsync(Guid accountId, decimal amount);
        ValueTask<Account> GetAccountByIdAsync(Guid accountId);
    }
}
