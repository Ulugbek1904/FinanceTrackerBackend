using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Foundations
{
    public class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;

        public UserService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var users = RetrieveAllUser();

            return await users.FirstOrDefaultAsync(user => user.Email == email);
        }

        public async ValueTask<User> ModifyUserAsync(User user)
        {
            var existingUser = await 
                this.storageBroker.SelectByIdAsync<User>(user.Id);

            if (existingUser == null)
                throw new UserNotFoundException();

            await this.storageBroker.UpdateAsync(user);

            return existingUser;

        }

        public async ValueTask<User> RegisterUserAsync(User user)
        {
            if (user == null) 
                throw new UserNullException();

            await this.storageBroker.InsertAsync(user);

            return user;
        }

        public async ValueTask<User> RemoveUserByIdAsync(Guid userId)
        {
            var existingUser = await RetrieveUserByIdAsync(userId);
            if (existingUser == null)
                throw new UserNotFoundException();

            return await this.storageBroker.DeleteAsync(existingUser);
        }

        public IQueryable<User> RetrieveAllUser()
        {
            var users = this.storageBroker.SelectAll<User>();

            if(users.Count() == 0)
                throw new Exception("User list is empty");

            return users;
        }

        public async ValueTask<User> RetrieveUserByIdAsync(Guid userId)
            => await this.storageBroker.SelectByIdAsync<User>(userId);

        public ValueTask<User> RetrieveUserByRefreshTokenAsync(string refreshToken)
        {
            var users = RetrieveAllUser();
            var user = users.FirstOrDefault(user => user.RefreshToken == refreshToken);
            if (user == null)
                throw new UserNotFoundException();
            return new ValueTask<User>(user);
        }

        public ValueTask<User> RetrieveUserByUsernameAsync(string firstName)
        {
            var users = RetrieveAllUser();
            var user = users.FirstOrDefault(user => user.FirstName == firstName);

            if (user == null)
                throw new UserNotFoundException();

            return new ValueTask<User>(user);
        }
    }
}
