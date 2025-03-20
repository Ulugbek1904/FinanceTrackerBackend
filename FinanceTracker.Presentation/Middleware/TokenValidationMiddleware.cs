using FinanceTracker.Infrastructure.Providers.AuthProvider;

namespace FinanceTracker.Presentation.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthProvider _authProvider;

        public TokenValidationMiddleware(RequestDelegate next, IAuthProvider authProvider)
        {
            _next = next;
            _authProvider = authProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var principal = _authProvider.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                }
            }

            await _next(context);
        }
    }
}
