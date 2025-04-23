using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Domain.Models.DTOs.AuthDtos
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
