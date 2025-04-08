using FinanceTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.Domain.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        public AccountType Source { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}
