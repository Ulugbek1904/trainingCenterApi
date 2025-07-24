using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserCreateDto dto);
        Task<List<UserDto>> RetrieveAllUsersAsync();
        Task<UserDto> RetrieveUserByIdAsync(Guid userId);
        Task<UserDto> ModifyUserAsync(UserUpdateDto userDto);
        Task<bool> RemoveUserAsync(Guid userId);
        Task<bool> ToggleUserStatusAsync(Guid id);
    }
}