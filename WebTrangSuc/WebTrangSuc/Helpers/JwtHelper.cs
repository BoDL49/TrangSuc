using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebTrangSuc.Helpers
{
    public static class JwtHelper
    {
        private static readonly string SecretKey = "95ff370ac8f19b2aa44379943f5aa18d7ba3c5d1e0c917072a78562a38de9bc1bb32a7f59f02389e61c6cba4cd9fe3c2b65635dff5aa08ec802a0f781cbb449845ad2e4ee7d7174eb6bbd084523a7cd947e862463d9fc70c195e3b90edb2e65e393fe385410fdc30726a12fd6eec9c5552865fea431a81ba175c1b8b86f507c6"; 
        private static readonly string Issuer = "WebTrangSuc";
        private static readonly string Audience = "WebTrangSucUsers";

        public static string GenerateToken(int userId, int role, int expireMinutes = 120)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero 
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
    }
}
