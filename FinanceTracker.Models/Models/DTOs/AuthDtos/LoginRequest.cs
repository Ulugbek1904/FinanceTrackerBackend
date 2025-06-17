namespace FinanceTracker.Domain.Models.DTOs.AuthDtos
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
