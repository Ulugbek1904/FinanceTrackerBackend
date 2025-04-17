using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Domain.Models
{
    public class Budget
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Limit amount must be greater than 0.")]
        public decimal LimitAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
