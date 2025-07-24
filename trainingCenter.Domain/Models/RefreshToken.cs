namespace trainingCenter.Domain.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpirationDate { get; set; }
    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

