using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Domain.Models.DTOs.BudgetDtos
{
    public class BudgetCreateDto
    {
        public string Name { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Limit amount must be greater than 0.")]
        public decimal LimitAmount { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
    }
}
