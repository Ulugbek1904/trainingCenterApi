using trainingCenter.Domain.Models.DTOs.Dashboard;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface ITenantDashboardService
{
    Task<TenantDashboardDto> GetTenantDashboardAsync(Guid tenantId);
}