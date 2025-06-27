using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public decimal? Discount { get; set; }
        public string InstallmentPlan { get; set; }
        public string ReceiptUrl { get; set; }
    }
}