using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; }
        public bool IsIncome { get; set; }
    }
}
