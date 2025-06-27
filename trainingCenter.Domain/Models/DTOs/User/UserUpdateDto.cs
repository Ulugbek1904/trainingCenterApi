using System;
using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        public string Password { get; set; } // Faqat yangilansa hash qilinadi
        [Required]
        public Role Role { get; set; }
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TelegramId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Address { get; set; }
        public string LanguagePreference { get; set; }
        public bool IsActive { get; set; }
    }
}