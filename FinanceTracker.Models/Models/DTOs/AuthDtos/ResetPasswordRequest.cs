namespace FinanceTracker.Domain.Models.DTOs.AuthDtos
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
    }
}
