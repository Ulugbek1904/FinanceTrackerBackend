using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;

namespace FinanceTracker.Services.Orchestrations
{
    public class UserOrchestration : IUserOrchestration
    {
        private readonly IUserService userService;
        private readonly IAccountService accountService;

        public UserOrchestration(
            IUserService userService,
            IAccountService accountService)
        {
            this.userService = userService;
            this.accountService = accountService;
        }
        public async ValueTask<User> RegisterUserAsync(User user)
        {
            var createdUser = await this.userService.RegisterUserAsync(user);
            
            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = createdUser.Id,
                Name = "My Wallet",
                Type = AccountType.Wallet,
                Balance = 0,
                IsPrimary = true
            };

            await this.accountService.CreateAccountAsync(newAccount);

            return createdUser;
        }
    }
}
