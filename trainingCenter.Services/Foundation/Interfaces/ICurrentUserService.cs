namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Guid TenantId { get; }
        string Role { get; }
        string Username { get; }
    }
}
