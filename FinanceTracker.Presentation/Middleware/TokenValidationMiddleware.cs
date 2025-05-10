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
            if (context.Request.Path.StartsWithSegments("/api/auth/login"))
            {
                await _next(context);
                return;
            }

            var excludedPaths = new[] { "/api/auth/login", "/api/auth/refresh-token" };
            if (excludedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
            {
                await _next(context);
                return;
            }

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
