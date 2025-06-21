using System.Text.Json.Serialization;

namespace trainingCenterApi.Common.Exceptions;

public class AppException : Exception
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; }

    [JsonPropertyName("type")]
    public string? Type { get; }

    [JsonPropertyName("message")]
    public override string Message => base.Message;

    public AppException(string message, int statusCode, string? type = null, Exception? inner = null)
        : base(message, inner)
    {
        if (!IsValidStatusCode(statusCode))
            throw new ArgumentException("Invalid HTTP status code", nameof(statusCode));
        StatusCode = statusCode;
        Type = type ?? $"https://httpstatuses.com/{statusCode}";
    }

    private bool IsValidStatusCode(int code) => code >= 400 && code < 600;
}

public static class ErrorTypes
{
    public const string UserNotFound = "UserNotFound";
    public const string CourseFull = "CourseFull";
    public const string InvalidPayment = "InvalidPayment";
    public const string StudentNotEnrolled = "StudentNotEnrolled";
    public const string DuplicateEntry = "DuplicateEntry";
}
