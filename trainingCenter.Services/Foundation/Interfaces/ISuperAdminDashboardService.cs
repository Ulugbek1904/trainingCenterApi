using trainingCenter.Domain.Models.DTOs.Dashboard;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface ISuperAdminDashboardService
{
    Task<SuperAdminDashboardDto> GetDashboardAsync();
}
