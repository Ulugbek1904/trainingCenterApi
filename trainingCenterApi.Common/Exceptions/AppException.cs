using System;

namespace trainingCenter.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string Type { get; }

        protected AppException(string message, int statusCode, string type)
            : base(message)
        {
            StatusCode = statusCode;
            Type = type ?? $"https://httpstatuses.com/{statusCode}";
        }

        protected AppException(string message, Exception innerException, int statusCode, string type)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Type = type ?? $"https://httpstatuses.com/{statusCode}";
        }
    }

    public class NullArgumentException : AppException
    {
        public NullArgumentException(string message)
            : base(message, 400, "https://httpstatuses.com/400")
        {
        }
        public NullArgumentException(string message, Exception innerException)
            : base(message, innerException, 400, "https://httpstatuses.com/400")
        {
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, 404, "https://httpstatuses.com/404")
        {
        }
    }

    public class ArgumentException : AppException
    {
        public ArgumentException(string message)
            : base(message, 400, "https://httpstatuses.com/400")
        {
        }
        public ArgumentException(string message, Exception innerException)
            : base(message, innerException, 400, "https://httpstatuses.com/400")
        {
        }
    }

    public class EmailException : AppException
    {
        public EmailException(string message, Exception innerException)
            : base(message, innerException, 500, "https://httpstatuses.com/500")
        {
        }
    }

    public class TokenValidationException : AppException
    {
        public TokenValidationException(string message, Exception innerException)
            : base(message, innerException, 401, "https://httpstatuses.com/401")
        {
        }
    }
    public class ValidationException : AppException
    {
        public ValidationException(string message)
            : base(message, 400, "https://httpstatuses.com/400")
        {
        }
    }

    public class ConfigurationException : AppException
    {
        public ConfigurationException(string message)
            : base(message, 500, "https://httpstatuses.com/500")
        {
        }
    }

    public class TelegramException : AppException
    {
        public TelegramException(string message, Exception innerException)
            : base(message, innerException, 500, "https://httpstatuses.com/500")
        {
        }
        public TelegramException(string message)
            : base(message, 500, "https://httpstatuses.com/500")
        {
        }
    }
}