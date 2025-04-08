

using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;

namespace FinanceTracker.Domain.Aggregates
{
    public class TransactionAggregate
    {
        public void ValidateTransaction(Transaction transaction)
        {
            ValidateAmount(transaction.Amount);
            ValidateTransactionSource(transaction.Source);
        }
        public void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new TransactionValidationException("Amount must be greater than 0");
            }
        }

        public void ValidateTransactionSource(AccountType source)
        {
            if (!Enum.IsDefined(typeof(AccountType), source))
            {
                throw new TransactionValidationException("Invalid Source");
            }
        }
    }
}
