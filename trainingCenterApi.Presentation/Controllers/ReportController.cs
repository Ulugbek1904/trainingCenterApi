using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Secretary,Teacher")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService reportService;
        private readonly ICurrentUserService currentUser;

        public ReportController(
            IReportService reportService,
            ICurrentUserService currentUser)
        {
            this.reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentReport(Guid studentId)
        {
            var report = await reportService.GetStudentReportAsync(
                studentId,
                tenantId: currentUser.TenantId);

            return Ok(report);
        }
    }
}
