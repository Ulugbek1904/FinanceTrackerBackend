using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Services.Foundations.Interfaces;
using System.Linq.Expressions;

namespace FinanceTracker.Services.Foundations
{
    public class AdminService : IAdminService
    {
        private readonly IUserService userService;

        public AdminService(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task BlockUserAsync(Guid userId)
        {
            var user = await this.userService.RetrieveUserByIdAsync(userId);
            user.IsActive = false;

            await this.userService.ModifyUserAsync(user);
        }

        public IQueryable<User> GetAllUsers(Expression<Func<User, bool>> filter)
        {
            return this.userService.RetrieveAllUser().Where(filter);
        }

        public async Task UnBlockUserAsync(Guid userId)
        {
            var user = await this.userService.RetrieveUserByIdAsync(userId);
            user.IsActive = true;

            await this.userService.ModifyUserAsync(user);
        }

        public async Task UpdateUserRoleAsync(Guid userId, Role role)
        {
            var user = await this.userService.RetrieveUserByIdAsync(userId);
            user.Role = role;

            await this.userService.ModifyUserAsync(user);
        }
    }
}
