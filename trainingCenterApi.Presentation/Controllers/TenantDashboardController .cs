using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Domain.Enums;

namespace trainingCenterApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(Role.Admin))]
public class TenantDashboardController : ControllerBase
{
    private readonly ITenantDashboardService dashboardService;
    private readonly ICurrentUserService currentUserService;

    public TenantDashboardController(
        ITenantDashboardService dashboardService,
        ICurrentUserService currentUserService)
    {
        this.dashboardService = dashboardService;
        this.currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardAsync()
    {
        var tenantId = currentUserService.TenantId;

        if (tenantId == null)
            return Unauthorized("TenantId aniqlanmadi");

        var result = await dashboardService.GetTenantDashboardAsync(tenantId);
        return Ok(result);
    }
}
