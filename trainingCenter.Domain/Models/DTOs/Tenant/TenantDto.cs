namespace trainingCenter.Domain.Models.DTOs.Tenant;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string Address { get; set; }
    public string Language { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}



