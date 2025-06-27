using System;
using System.Threading.Tasks;
using trainingCenter.Domain.Dtos;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IReportService
    {
        Task<StudentReportDto> GetStudentReportAsync(
            Guid studentId, DateTime? startDate = null, DateTime? endDate = null);
    }
}