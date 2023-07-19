using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AppCore.Extensions
{
    public static class JwtExtensions
    {
        public static string GenerateAccessToken(IEnumerable<Claim> claims, DateTime expiredAt)
        {
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvironmentExtension.GetJwtAccessTokenSecret()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                EnvironmentExtension.GetJwtIssuer(),
                EnvironmentExtension.GetJwtAudience(),
                claims,
                DatetimeExtension.UtcNow(),
                expiredAt,
                credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static IEnumerable<Claim> ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = EnvironmentExtension.GetJwtIssuer(),
                    ValidAudience = EnvironmentExtension.GetJwtAudience(),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(EnvironmentExtension.GetJwtAccessTokenSecret())
                    ),
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Claims;
            }
            catch
            {
                return new List<Claim>();
            }
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return $"{Convert.ToBase64String(randomNumber)}{Guid.NewGuid():N}";
        }
    }
}