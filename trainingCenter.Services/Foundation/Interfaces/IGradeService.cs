using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IGradeService
    {
        Task<Grade> RegisterGradeAsync(Grade grade);
        Task<List<Grade>> RetrieveAllGradesAsync();
        Task<Grade> RetrieveGradeByIdAsync(Guid gradeId);
        Task<Grade> ModifyGradeAsync(Grade grade);
        Task<Grade> RemoveGradeAsync(Guid gradeId);
    }
}