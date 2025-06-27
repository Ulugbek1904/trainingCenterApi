using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;

        public UserService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ArgumentException("User cannot be null or have empty Username/Password");

            if (await storageBroker.SelectAll<User>().AnyAsync(u => u.Username == user.Username))
                throw new ArgumentException("Username already exists");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            return await storageBroker.InsertAsync(user);
        }

        public async Task<List<User>> RetrieveAllUsersAsync()
        {
            return await storageBroker.SelectAll<User>().ToListAsync();
        }

        public async Task<User> RetrieveUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty");

            var user = await storageBroker.SelectByIdAsync<User>(userId);
            return user ?? throw new NotFoundException($"User with ID {userId} not found");
        }

        public async Task<User> ModifyUserAsync(User user)
        {
            if (user.Id == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty");

            var existing = await storageBroker.SelectByIdAsync<User>(user.Id);
            if (existing == null)
                throw new NotFoundException($"User with ID {user.Id} not found");

            if (!string.IsNullOrWhiteSpace(user.PasswordHash) && user.PasswordHash != existing.PasswordHash)
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            return await storageBroker.UpdateAsync(user);
        }

        public async Task<User> RemoveUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty");

            var user = await storageBroker.SelectByIdAsync<User>(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found");

            return await storageBroker.DeleteAsync(user);
        }
    }
}