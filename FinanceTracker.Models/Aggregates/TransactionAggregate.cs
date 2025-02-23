

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
            ValidateCategory(transaction.Category.Type);
            ValidateTransactionSource(transaction.Source);
        }
        public void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new TransactionValidationException("Amount must be greater than 0");
            }
        }

        public void ValidateCategory(CategoryType category)
        {
            if (!Enum.IsDefined(typeof(CategoryType), category))
            {
                throw new TransactionValidationException("Invalid Category");
            }
        }

        public void ValidateTransactionSource(TransactionSource source)
        {
            if (!Enum.IsDefined(typeof(TransactionSource), source))
            {
                throw new TransactionValidationException("Invalid Source");
            }
        }
    }
}
