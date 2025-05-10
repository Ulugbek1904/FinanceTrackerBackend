namespace FinanceTracker.Domain.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string? Type { get; }

        public AppException(string message, int statusCode = 400,
            string? type = null, Exception? inner = null)
                : base(message)
        {
            StatusCode = statusCode;
            Type = type ?? $"https://httpstatuses.com/{statusCode}";
        }
    }

}
