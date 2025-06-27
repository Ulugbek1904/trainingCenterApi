using System;
using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class PaymentCreateDto
    {
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        [StringLength(100)]
        public string TransactionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; }
        public string InstallmentPlan { get; set; }
        [StringLength(500)]
        public string ReceiptUrl { get; set; }
    }
}