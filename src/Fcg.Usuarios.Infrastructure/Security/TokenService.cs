using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Token;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Enum;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fcg.Usuarios.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings  = jwtSettings.Value;
        }
        public async Task<TokenResult> GerarToken(UsuarioResponse usuario)
        {
            
            var claims = await ObtemClaims(usuario);
            var acessToken = ObtemToken(claims);
            return new TokenResult
            {
                AccessToken = acessToken,
                ExpiresIn = _jwtSettings.ExpiracaoHoras * 3600,
                Claims = claims.Select(c => new ClaimResponse { Type = c.Type, Value = c.Value })
            };
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


        public string ObtemToken(IEnumerable<Claim> claims)
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
    }
}
