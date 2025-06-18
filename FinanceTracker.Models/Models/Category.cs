using FinanceTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanceTracker.Domain.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsIncome { get; set; }
        public bool IsDefault { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        [JsonIgnore]
        public List<Transaction> Transactions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
