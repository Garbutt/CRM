using CRM.Server.Models;


namespace CRM.Server.Helpers
{
    public class isUserAdmin
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public isUserAdmin(ILogger<isUserAdmin> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool checkAdmin()
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

                _logger.LogError("Admin role required to add site.");
                return tokenClaim.role ==  "admin";

        }
    }
}
