using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models.DTOs.Student;

public class StudentUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }

    [Phone]
    public string ParentPhoneNumber { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public string Address { get; set; }

    public bool IsActive { get; set; }
    public string Notes { get; set; }
}
