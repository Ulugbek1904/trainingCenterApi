using System.Threading.Tasks;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Notification notification);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<List<Notification>> GetNotificationsByStudentIdAsync(Guid tenantId, Guid studentId);
        Task<List<Notification>> GetUndeliveredNotificationsAsync(Guid tenantId);
        Task SendGroupNotificationAsync(
            Guid tenantId,
            string message,
            NotificationType type,
            NotificationPriority priority,
            int? categoryId = null,
            Guid? courseId = null);
    }
}