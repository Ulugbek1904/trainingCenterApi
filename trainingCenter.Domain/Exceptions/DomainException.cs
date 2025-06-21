using trainingCenterApi.Common.Exceptions;

namespace trainingCenter.Domain.Exceptions;

public class DomainException : AppException
{
    public DomainException(string message, int statusCode = 400, string? type = null)
        : base(message, statusCode, type)
    {
    }
}
