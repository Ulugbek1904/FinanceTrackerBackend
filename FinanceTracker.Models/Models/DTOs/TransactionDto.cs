using FinanceTracker.Domain.Enums;
using CsvHelper.Configuration.Attributes;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class TransactionDto
    {
        [Name("Nima uchun ishlatilgani")]
        public string Description { get; set; }
        [Name("Miqdori")]
        public decimal Amount { get; set; }
        [Name("Sana")]
        public DateTime TransactionDate { get; set; }
        [Name("Hisob raqami")]
        public TransactionType TransactionType { get; set; }
        [Name("Kategoriya")]
        public string CategoryName { get; set; }
        [Name("Hisob raqami nomi")]
        public string AccountName { get; set; }
    }
}
