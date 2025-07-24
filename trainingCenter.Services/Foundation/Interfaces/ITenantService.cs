using trainingCenter.Domain.Models.DTOs.Tenant;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface ITenantService
{
    Task<TenantDto> CreateTenantAsync(TenantCreateDto dto);
    Task<TenantDto> UpdateTenantAsync(Guid id, TenantUpdateDto dto);
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
    Task<TenantDto> GetTenantByIdAsync(Guid id);
    Task<bool> DeleteTenantAsync(Guid id);
    Task<bool> ToggleTenantStatusAsync(Guid id); 
}
