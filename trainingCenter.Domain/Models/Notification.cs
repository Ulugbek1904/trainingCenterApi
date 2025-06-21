using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid? StudentId { get; set; }
    public Student Student { get; set; }
    public string RecipientTelegramId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsDelivered { get; set; }
    public NotificationChannel Channel { get; set; } 
    public NotificationPriority Priority { get; set; }
}
