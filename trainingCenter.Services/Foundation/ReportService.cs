using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Dtos;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Services.Foundation
{
    public class ReportService : IReportService
    {
        private readonly IStorageBroker storageBroker;

        public ReportService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        }

        public async Task<StudentReportDto> GetStudentReportAsync(
           Guid studentId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var student = await storageBroker.SelectByIdAsync<Student>(studentId);
            if (student == null)
                throw new NotFoundException($"Student with ID {studentId} not found");

            var attendanceQuery = storageBroker.SelectAll<Attendance>()
                .Where(a => a.StudentId == studentId)
                .Include(a => a.Course)
                .AsQueryable();

            var gradeQuery = storageBroker.SelectAll<Grade>()
                .Where(g => g.StudentId == studentId)
                .Include(g => g.Course)
                .AsQueryable();

            var paymentQuery = storageBroker.SelectAll<Payment>()
                .Where(p => p.StudentId == studentId)
                .Include(p => p.Course)
                .AsQueryable();

            if (startDate.HasValue)
            {
                attendanceQuery = attendanceQuery.Where(a => a.Date >= startDate.Value);
                gradeQuery = gradeQuery.Where(g => g.Date >= startDate.Value);
                paymentQuery = paymentQuery.Where(p => p.PaymentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                attendanceQuery = attendanceQuery.Where(a => a.Date <= endDate.Value);
                gradeQuery = gradeQuery.Where(g => g.Date <= endDate.Value);
                paymentQuery = paymentQuery.Where(p => p.PaymentDate <= endDate.Value);
            }

            var attendances = await attendanceQuery.ToListAsync();
            var grades = await gradeQuery.ToListAsync();
            var payments = await paymentQuery.ToListAsync();

            var report = new StudentReportDto
            {
                FullName = student.FullName,
                AttendanceHistory = attendances.Select(a => new AttendanceReportDto
                {
                    Date = a.Date,
                    CourseName = a.Course.Name,
                    Status = a.Status.ToString(),
                    Comment = a.Notes ?? "Yo'q"
                }).ToList(),
                GradeHistory = grades.Select(g => new GradeReportDto
                {
                    Date = g.Date,
                    CourseName = g.Course.Name,
                    Score = g.Score,
                    Comment = g.Comment ?? "Yo'q"
                }).ToList(),
                PaymentHistory = payments.Select(p => new PaymentReportDto
                {
                    PaymentDate = p.PaymentDate,
                    DueDate = p.DueDate,
                    Amount = p.Amount,
                    Status = p.Status,
                    CourseName = p.Course.Name,
                    InstallmentPlan = p.InstallmentPlan ?? "Yo'q"
                }).ToList(),
                TotalAttendanceCount = attendances.Count(a => a.Status == Status.keldi),
                TotalAttendanceHours = attendances.Count(a => a.Status == Status.keldi) * 2
            };

            return report;
        }
    }
}