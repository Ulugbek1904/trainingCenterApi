using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs.PageDto;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenterApi.Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminDashboardController : ControllerBase
{
    private readonly ISuperAdminDashboardService superAdminDashboard;
    private readonly IStudentService studentService;

    public SuperAdminDashboardController(
        ISuperAdminDashboardService superAdminDashboard,
        IStudentService studentService)
    {
        this.superAdminDashboard = superAdminDashboard;
        this.studentService = studentService;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatisticsAsync()
    {
        var statistics = await superAdminDashboard.GetDashboardAsync();
        if (statistics == null)
        {
            return NotFound("Statistics not found.");
        }
        return Ok(statistics);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudentStatisticsAsync([FromQuery] StudentQueryDto query)
    {
        var students = await studentService.RetrieveAllStudents();

        if (!string.IsNullOrEmpty(query.FullName))
        {
            students = students
                .Where(s => s.FullName.Contains(query.FullName, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Gender.HasValue)
        {
            students = students
                .Where(s => s.Gender == query.Gender.Value);
        }

        if (query.StartYear.HasValue)
        {
            students = students
                .Where(s => s.BirthDate >= query.StartYear.Value);
        }

        if (query.EndYear.HasValue)
        {
            students = students
                .Where(s => s.BirthDate <= query.EndYear.Value);
        }

        if (!string.IsNullOrEmpty(query.Address))
        {
            students = students
                .Where(s => s.Address.Contains(query.Address, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(students);
    }
}
