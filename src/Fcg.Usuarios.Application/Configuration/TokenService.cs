using Fcg.Usuarios.Application.Dtos;
using Fcg.Usuarios.Application.Services.Interfaces;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Infrastructure.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fcg.Usuarios.Application.Configuration
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtSettings,
            ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }
        public async Task<LoginResponse> RetornaJwt(UsuarioResponse usuario)
        {
            _logger.LogInformation("Iniciando geração de token. ID: {id}", usuario.Id);
            var claims = await ObtemClaims(usuario);
            var acessToken = TokenGenerate(claims);
            _logger.LogInformation("Token gerado com sucesso. ID: {id}", usuario.Id);
            return new LoginResponse
            {
                AcessToken = acessToken,
                ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpiracaoHoras).TotalSeconds,
                Id = usuario.Id.ToString(),
                Email = usuario.Email,
                Claims = claims.Select(c => new ClaimResponse { Type = c.Type, Value = c.Value })
            };
        }

        public string TokenGenerate(IEnumerable<Claim> claims)
        {
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Emissor,
                Audience = _jwtSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(tokenDescriptor);
        }

        public async Task<IEnumerable<Claim>> ObtemClaims(UsuarioResponse usuario)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, usuario.Nome));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, usuario.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            if (usuario.PerfilUsuario == TipoUsuario.Administrador) claims.Add(new Claim(ClaimTypes.Role, "AdminRole"));
            if (usuario.PerfilUsuario == TipoUsuario.Jogador) claims.Add(new Claim(ClaimTypes.Role, "JogadorRole"));
            return claims;
        }



        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
