using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Models.DTOs.Tenant;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenterApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class TenantController : ControllerBase
{
    private readonly ITenantService tenantService;

    public TenantController(ITenantService tenantService)
    {
        this.tenantService = tenantService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(TenantCreateDto dto) =>
        Ok(await tenantService.CreateTenantAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, TenantUpdateDto dto) =>
        Ok(await tenantService.UpdateTenantAsync(id, dto));

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await tenantService.GetAllTenantsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await tenantService.GetTenantByIdAsync(id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        Ok(await tenantService.DeleteTenantAsync(id));

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id) =>
        Ok(await tenantService.ToggleTenantStatusAsync(id));
}
