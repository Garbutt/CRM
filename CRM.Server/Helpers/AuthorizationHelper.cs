using CRM.Server.Models;

namespace CRM.Server.Helpers
{
    public class AuthorizationHelper
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHelper(ILogger<AuthorizationHelper> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool checkAuth()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                _logger.LogError("Http context is null");
                return false;
            }

            string? authorizationHeader = httpContext.Request.Headers["authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                _logger.LogError("Headers are empty");
                return false;
            }

            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Token does not start with bearer");
                return false;
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);

            if (tokenClaim == null)
            {
                _logger.LogError("Token is null here.");
                return false;
            }

            return true;
        }
    }
}
