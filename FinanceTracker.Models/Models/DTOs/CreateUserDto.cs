using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class CreateUserDto
    {
        [EmailAddress(ErrorMessage ="Email is not valid")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
