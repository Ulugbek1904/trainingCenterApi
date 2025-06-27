using System;
using System.Collections.Generic;
using trainingCenter.Domain.Models;

namespace trainingCenter.Domain.Dtos
{
    public class StudentReportDto
    {
        public string FullName { get; set; }
        public List<AttendanceReportDto> AttendanceHistory { get; set; }
        public List<GradeReportDto> GradeHistory { get; set; }
        public List<PaymentReportDto> PaymentHistory { get; set; }
        public int TotalAttendanceCount { get; set; }
        public int TotalAttendanceHours { get; set; }
    }

    public class AttendanceReportDto
    {
        public DateTime Date { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }

    public class GradeReportDto
    {
        public DateTime Date { get; set; }
        public string CourseName { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
    }

    public class PaymentReportDto
    {
        public DateTime PaymentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string CourseName { get; set; }
        public string InstallmentPlan { get; set; }
    }
}