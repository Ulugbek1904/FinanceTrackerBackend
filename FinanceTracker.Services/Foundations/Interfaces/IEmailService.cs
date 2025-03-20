namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IEmailService
    {
        ValueTask SendPasswordResetEmailAsync(string email);
        ValueTask ResetPasswordAsync(string email, string otpCode, string newPassword);
    }
}
