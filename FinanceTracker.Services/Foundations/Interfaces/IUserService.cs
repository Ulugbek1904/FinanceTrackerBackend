using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IUserService
    {
        ValueTask<User> RegisterUserAsync(User user);
        ValueTask<User> RetrieveUserByIdAsync(Guid userId);
        ValueTask<User> ModifyUserAsync(User user);
        ValueTask<User> RemoveUserByIdAsync(Guid userId);
        IQueryable<User> RetrieveAllUser();
        ValueTask<User> RetrieveUserByUsernameAsync(string firstName);
        Task<User> GetUserByEmailAsync(string email);
    }
}
