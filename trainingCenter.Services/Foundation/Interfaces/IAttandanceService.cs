using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IAttendanceService
    {
        Task<Attendance> RegisterAttendanceAsync(Attendance attendance);
        Task<List<Attendance>> RetrieveAllAttendancesAsync();
        Task<Attendance> RetrieveAttendanceByIdAsync(Guid attendanceId);
        Task<Attendance> ModifyAttendanceAsync(Attendance attendance);
        Task<Attendance> RemoveAttendanceAsync(Guid attendanceId);
    }
}