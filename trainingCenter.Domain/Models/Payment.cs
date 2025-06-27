using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    [Required, Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime? DueDate { get; set; } 
    public string? Status { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public decimal? Discount { get; set; } 
    public string? InstallmentPlan { get; set; } 
    public string? ReceiptUrl { get; set; }
}
