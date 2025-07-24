using AutoMapper;
using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IMapper mapper;

        public UserService(IStorageBroker storageBroker, IMapper mapper)
        {
            this.storageBroker = storageBroker;
            this.mapper = mapper;
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto dto)
        {
            if (await storageBroker.SelectAll<User>().AnyAsync(u => u.Username == dto.Username))
                throw new ArgumentException("Username already exists");

            if (dto.Role == Role.SuperAdmin)
                throw new ValidationException("SuperAdmin foydalanuvchi yaratish mumkin emas");

            var tenant = await storageBroker.SelectByIdAsync<Tenant>(dto.TenantId)
                         ?? throw new NotFoundException("Tenant not found");

            var user = mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CreatedAt = DateTime.UtcNow;

            await storageBroker.InsertAsync(user);
            return mapper.Map<UserDto>(user);
        }


        public async Task<List<UserDto>> RetrieveAllUsersAsync()
        {
            var users = await storageBroker.SelectAll<User>().ToListAsync();
            return mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> RetrieveUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty");

            var user = await storageBroker.SelectByIdAsync<User>(userId)
                       ?? throw new NotFoundException($"User with ID {userId} not found");

            return mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> ModifyUserAsync(UserUpdateDto userDto)
        {
            if (userDto.Id == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty");

            var existingUser = await storageBroker.SelectByIdAsync<User>(userDto.Id)
                                 ?? throw new NotFoundException($"User with ID {userDto.Id} not found");

            existingUser.FullName = userDto.FullName;
            existingUser.PhoneNumber = userDto.PhoneNumber;
            existingUser.Role = userDto.Role;

            if (!string.IsNullOrWhiteSpace(userDto.Password))
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            await storageBroker.UpdateAsync(existingUser);
            return mapper.Map<UserDto>(existingUser);
        }

        public async Task<bool> RemoveUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty");

            var user = await storageBroker.SelectByIdAsync<User>(userId)
                       ?? throw new NotFoundException($"User with ID {userId} not found");

            await storageBroker.DeleteAsync(user);
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(Guid id)
        {
            var user = await storageBroker.SelectByIdAsync<User>(id)
                        ?? throw new NotFoundException("User not found");

            user.IsActive = !user.IsActive;

            await storageBroker.UpdateAsync(user);

            return user.IsActive;
        }
    }
}
