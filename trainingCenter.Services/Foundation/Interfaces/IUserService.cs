using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user);
        Task<List<User>> RetrieveAllUsersAsync();
        Task<User> RetrieveUserByIdAsync(Guid userId);
        Task<User> ModifyUserAsync(User user);
        Task<User> RemoveUserAsync(Guid userId);
    }
}