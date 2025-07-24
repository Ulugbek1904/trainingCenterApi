using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models.DTOs.Notification;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService notificationService;
    private readonly ICurrentUserService currentUser;

    public NotificationController(
        INotificationService notificationService,
        ICurrentUserService currentUser)
    {
        this.notificationService = notificationService
            ?? throw new NullArgumentException(nameof(notificationService));
        this.currentUser = currentUser
            ?? throw new NullArgumentException(nameof(currentUser));
    }

    [HttpPost("send-group")]
    public async Task<IActionResult> SendGroupNotification([FromBody] GroupNotificationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest("Message cannot be empty.");

        await notificationService.SendGroupNotificationAsync(
            tenantId: currentUser.TenantId,
            message: dto.Message,
            type: dto.Type,
            priority: dto.Priority,
            categoryId: dto.CategoryId,
            courseId: dto.CourseId);

        return Ok("Group notification sent.");
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentNotifications(Guid studentId)
    {
        if (studentId == Guid.Empty)
            return BadRequest("Invalid student ID.");

        var notifications = await notificationService.GetNotificationsByStudentIdAsync(
            tenantId: currentUser.TenantId,
            studentId: studentId);

        return Ok(notifications);
    }

    [HttpGet("unread")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUndeliveredNotifications()
    {
        var notifications = await notificationService.GetUndeliveredNotificationsAsync(
            tenantId: currentUser.TenantId);

        return Ok(notifications);
    }

}
