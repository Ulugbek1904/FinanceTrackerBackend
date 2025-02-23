using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }
    }
}
