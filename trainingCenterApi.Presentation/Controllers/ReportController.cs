using Microsoft.AspNetCore.Mvc;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService reportService;

        public ReportController(IReportService reportService)
        {
            this.reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentReport(Guid studentId)
        {
            var report = await reportService.GetStudentReportAsync(studentId);
            return Ok(report);
        }
    }
}