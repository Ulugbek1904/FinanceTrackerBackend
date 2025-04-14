using FinanceTracker.Infrastructure.Brokers.Email;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;

namespace FinanceTracker.Services.Foundations
{
    public class EmailService : IEmailService
    {
        private readonly IEmailBroker emailBroker;
        private readonly IUserService userService;

        public EmailService(
            IEmailBroker emailBroker,
            IUserService userService)
        {
            this.emailBroker = emailBroker;
            this.userService = userService;
        }

        public async ValueTask ResetPasswordAsync(string email, string otpCode, string newPassword)
        {
            var user = await this.userService.GetUserByEmailAsync(email);

            if (user == null || 
                user.OtpExpiration < DateTime.UtcNow || 
                otpCode != user.PasswordResetOtp)
                throw new UnauthorizedAccessException("Invalid code");

            user.HashedPassword = newPassword;
            user.OtpExpiration = null;
            user.PasswordResetOtp = null;

            await userService.ModifyUserAsync(user);
        }

        public async ValueTask SendPasswordResetEmailAsync(string email)
        {
            var user = await this.userService.GetUserByEmailAsync(email);
            if (user == null) return; // bu xavfni oldini olish uchun ya'ni bunday user bormi yo'qmi tekshirishni oldini oladi 

            string optCode = new Random().Next(100000,999999).ToString();
            user.PasswordResetOtp = optCode;
            user.OtpExpiration = DateTime.UtcNow.AddMinutes(30);

            await this.userService.ModifyUserAsync(user);

            await this.emailBroker.SendPasswordResetEmailAsync(email, optCode);
        }
    }
}
