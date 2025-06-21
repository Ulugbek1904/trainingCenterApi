using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; }
    public Role Role { get; set; }
    public string FullName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string TelegramId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
    public DateTime? LastLoginAt { get; set; } = null; 
    public bool IsActive { get; set; } 
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string LanguagePreference { get; set; } = string.Empty;
}
