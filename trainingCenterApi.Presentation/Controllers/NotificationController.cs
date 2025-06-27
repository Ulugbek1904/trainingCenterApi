using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService 
                ?? throw new NullArgumentException(nameof(notificationService));
        }

        [HttpPost("send-group")]
        public async Task<IActionResult> SendGroupNotification([FromBody] GroupNotificationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Message cannot be empty.");

            await notificationService.SendGroupNotificationAsync(
                dto.Message,
                dto.Type,
                dto.Priority,
                dto.CategoryId,
                dto.CourseId);

            return Ok("Group notification sent.");
        }
    }

    public class GroupNotificationDto
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; }
        public int? CategoryId { get; set; }
        public Guid? CourseId { get; set; }
    }
}