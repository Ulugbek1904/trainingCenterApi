using System;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TelegramId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Address { get; set; }
        public string LanguagePreference { get; set; }
    }
}