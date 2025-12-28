using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartSurfSpots.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartSurfSpots.Services.Helpers
{
    /// <summary>
    /// Classe utilitária responsável pela gestão de Tokens JWT (JSON Web Tokens).
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um novo token JWT para um utilizador autenticado.
        /// </summary>
        /// <param name="user">O utilizador para o qual o token será gerado.</param>
        /// <returns>A string do token encriptado.</returns>
        public string GenerateToken(User user)
        {
            var keyContent = _configuration["JwtSettings:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyContent));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims são os dados que vão "dentro" do token (ID, Nome, Email)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único do token
            };

            var expiryHours = Convert.ToDouble(_configuration["JwtSettings:ExpiryInHours"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Valida um token recebido e extrai o ID do utilizador se for válido.
        /// </summary>
        /// <param name="token">A string do token JWT.</param>
        /// <returns>O ID do utilizador (int) ou null se o token for inválido/expirado.</returns>
        public int? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true, // Garante que não está expirado
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                return userId;
            }
            catch
            {
                // Se falhar a validação (expirado, assinatura errada, etc.), retorna null
                return null;
            }
        }
    }
}