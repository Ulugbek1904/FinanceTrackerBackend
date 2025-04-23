namespace FinanceTracker.Domain.Models.DTOs.AuthDtos
{
    public class LoginRequest
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
