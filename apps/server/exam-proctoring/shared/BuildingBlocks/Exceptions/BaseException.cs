namespace BuildingBlocks.Exceptions
{
    public class BaseException : Exception
    {
        public int StatusCode { get; }
        public string? ErrorCode { get; }
        public object? Details { get; }

        public BaseException(string message, int statusCode = 500, string? errorCode = null, object? details = null) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }
    }
}