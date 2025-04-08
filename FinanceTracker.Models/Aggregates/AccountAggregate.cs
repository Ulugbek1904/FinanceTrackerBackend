using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;

namespace FinanceTracker.Domain.Aggregates
{
    public class AccountAggregate
    {
        public void ValidateAccount(Account account)
        {
            ValidateName(account.Name);
            ValidateBalance(account.Type, account.Balance);
        }
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new AccountValidationException("Account name is required");
            }
        }

        private void ValidateBalance(AccountType source, decimal balance)
        {
            if (source is AccountType.Cash or AccountType.DebitCard && balance < 0)
            {
                throw new AccountValidationException($"{source} account balance cannot be negative");
            }
        }

    }
}
