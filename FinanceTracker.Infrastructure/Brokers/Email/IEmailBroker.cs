namespace FinanceTracker.Infrastructure.Brokers.Email
{
    public interface IEmailBroker
    {
        ValueTask SendPasswordResetEmailAsync(string email, string PasswordResetOtp);
    }
}