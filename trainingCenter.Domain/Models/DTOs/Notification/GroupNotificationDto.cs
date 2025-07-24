using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs.Notification;

public class GroupNotificationDto
{
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public int? CategoryId { get; set; }
    public Guid? CourseId { get; set; }
}
