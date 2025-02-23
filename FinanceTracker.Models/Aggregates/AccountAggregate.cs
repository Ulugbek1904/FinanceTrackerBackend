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
            ValidateBalance(account.Source, account.Balance);
        }
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new AccountValidationException("Account name is required");
            }
        }

        private void ValidateBalance(TransactionSource source, decimal balance)
        {
            if (source is TransactionSource.Cash or TransactionSource.DebitCard && balance < 0)
            {
                throw new AccountValidationException($"{source} account balance cannot be negative");
            }
        }

    }
}
