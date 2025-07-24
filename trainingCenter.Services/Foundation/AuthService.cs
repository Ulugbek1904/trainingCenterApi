using Microsoft.EntityFrameworkCore;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs.Auth;
using trainingCenter.Domain.Models.DTOs.User;
using trainingCenter.Common.Exceptions;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Infrastructure.providers.AuthProvider;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenterApi.Common.HelperFunctions;
using Serilog;

public class AuthService : IAuthService
{
    private readonly IStorageBroker storageBroker;
    private readonly IAuthProvider authProvider;
    private readonly IPasswordHasher passwordHasher;

    public AuthService(
        IStorageBroker storageBroker,
        IAuthProvider authProvider,
        IPasswordHasher passwordHasher)
    {
        this.storageBroker = storageBroker;
        this.authProvider = authProvider;
        this.passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> LoginAsync(LoginDto loginDto)
    {
        Log.Information("Login attempt: Identifier={Identifier}", loginDto.Identifier);

        if (string.IsNullOrWhiteSpace(loginDto.Identifier) || string.IsNullOrWhiteSpace(loginDto.Password))
            throw new ValidationException("Username yoki telefon raqam va parol to‘ldirilishi shart.");

        var user = await storageBroker.SelectAll<User>()
            .Include(u => u.RefreshTokens)
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u =>
                u.Username == loginDto.Identifier);

        if (user == null || !user.IsActive)
        {
            Log.Error("User not found or inactive: Identifier={Identifier}", loginDto.Identifier);
            throw new NotFoundException("Foydalanuvchi topilmadi yoki bloklangan.");
        }

        Log.Information("User found: Id={UserId}, TenantId={TenantId}, TenantIsActive={TenantIsActive}, RefreshTokensCount={RefreshTokensCount}",
            user.Id, user.TenantId, user.Tenant?.IsActive, user.RefreshTokens?.Count ?? 0);

        if (!passwordHasher.Verify(loginDto.Password, user.PasswordHash))
        {
            Log.Error("Invalid password for user: {Identifier}", loginDto.Identifier);
            throw new ValidationException("Parol noto‘g‘ri.");
        }

        if (user.Tenant == null)
        {
            Log.Error("Tenant is null for user: {UserId}", user.Id);
            throw new InvalidOperationException("Tenant object is null for this user.");
        }

        if (!user.Tenant.IsActive)
        {
            Log.Error("Tenant is inactive for user: {UserId}", user.Id);
            throw new ValidationException("This educational center is currently deactivated.");
        }

        var accessToken = authProvider.GenerateJwtToken(user);
        var (refreshTokenStr, refreshTokenExp) = authProvider.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenStr,
            ExpirationDate = refreshTokenExp,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.RefreshTokens.Add(refreshToken);

        Log.Information("Updating user: Id={UserId}, LastLoginAt={LastLoginAt}", user.Id, user.LastLoginAt);
        await storageBroker.UpdateAsync(user);
        Log.Information("User updated successfully: Id={UserId}", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            User = new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                Username = user.Username,
                Role = user.Role,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                TelegramId = user.TelegramId,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Address = user.Address,
                LanguagePreference = user.LanguagePreference
            }
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refrshToken)
    {
        if (string.IsNullOrWhiteSpace(refrshToken))
            throw new ValidationException("Refresh token bo‘sh bo‘lishi mumkin emas.");

        var userWithToken = await storageBroker.SelectAll<User>()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r =>
                r.Token == refrshToken && !r.IsRevoked));

        if (userWithToken == null)
            throw new NotFoundException("Refresh token topilmadi yoki yaroqsiz.");


        var existingToken = userWithToken.RefreshTokens
            .First(r => r.Token == refrshToken && !r.IsRevoked);

        if (existingToken.ExpirationDate < DateTime.UtcNow)
            throw new ValidationException("Refresh token muddati tugagan.");

        existingToken.IsRevoked = true;
        existingToken.UpdatedAt = DateTime.UtcNow;

        var newAccessToken = authProvider.GenerateJwtToken(userWithToken);
        var (newRefreshTokenStr, newRefreshExp) = authProvider.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            UserId = userWithToken.Id,
            Token = newRefreshTokenStr,
            ExpirationDate = newRefreshExp,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        userWithToken.RefreshTokens.Add(newRefreshToken);
        userWithToken.UpdatedAt = DateTime.UtcNow;

        await storageBroker.UpdateAsync(userWithToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenStr,
            User = new UserDto
            {
                Id = userWithToken.Id,
                Username = userWithToken.Username,
                Role = userWithToken.Role,
                FullName = userWithToken.FullName,
                Email = userWithToken.Email,
                PhoneNumber = userWithToken.PhoneNumber,
                TelegramId = userWithToken.TelegramId,
                CreatedAt = userWithToken.CreatedAt,
                LastLoginAt = userWithToken.LastLoginAt,
                IsActive = userWithToken.IsActive,
                ProfilePictureUrl = userWithToken.ProfilePictureUrl,
                Address = userWithToken.Address,
                LanguagePreference = userWithToken.LanguagePreference
            }
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var userWithToken = await storageBroker.SelectAll<User>()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken && !r.IsRevoked));

        if (userWithToken == null)
            throw new NotFoundException("Refresh token not found.");

        var token = userWithToken.RefreshTokens.First(r => r.Token == refreshToken);
        token.IsRevoked = true;
        token.UpdatedAt = DateTime.UtcNow;

        await storageBroker.UpdateAsync(userWithToken);
    }


}
