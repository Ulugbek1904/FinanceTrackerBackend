using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using System.Linq.Expressions;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IAdminService
    {
        IQueryable<User> GetAllUsers(Expression<Func<User,bool>> filter);
        Task BlockUserAsync(Guid userId);
        Task UpdateUserRoleAsync(Guid userId, Role role);
        Task UnBlockUserAsync(Guid userId);
    }
}
