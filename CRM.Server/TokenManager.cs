
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CRM.Server.Models;

namespace CRM.Server
{
    public class TokenManager
    {
        public static string Secret = "helloWorldowowoddmdmdmdowowowowmdmdmdm123mdmdmdm321kdkdkdkdmcmcmcmdkkdkddkcmmcmckdldldl";

        public static string GenerateToken(string email, string role)
        {
            SymmetricSecurityKey secuityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Role, role) }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(secuityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    return null;
                }
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret))
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static TokenClaim ValidateToken(string token, ILogger logger)
        {

            if (string.IsNullOrEmpty(token))
            {
                logger.LogError("Token is empty ");
                return null;
            }

            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
            {
                logger.LogWarning("Principal is null");
                return null;
            }
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (Exception ex)
            {
                logger.LogError("Identity is null");
                return null;
            }
            // Initialize new TokenClaim object
            TokenClaim tokenClaim = new TokenClaim();
            
            var emailClaim = identity.FindFirst(ClaimTypes.Email);
            var roleClaim = identity.FindFirst(ClaimTypes.Role);
           
            if(emailClaim != null && roleClaim != null)
            {
                tokenClaim.email = emailClaim.Value;
                tokenClaim.role = roleClaim.Value;
            }
            else
            {
                logger.LogWarning("Email or role claim is null");
                return null;
            }
            return tokenClaim;
        }
    }
}