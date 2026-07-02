using Fcg.Core.WebApi.Security;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Application.Features.Users.Responses.Token;
using Fcg.Users.Domain.Enum;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fcg.Users.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings  = jwtSettings.Value;
        }
        public async Task<TokenResult> GerarToken(UserResponse User)
        {            
            var claims = await ObtemClaims(User);
            var acessToken = ObtemToken(claims);
            return new TokenResult
            {
                AccessToken = acessToken,
                ExpiresIn = _jwtSettings.ExpirationHours * 3600,
                Claims = claims.Select(c => new ClaimResponse { Type = c.Type, Value = c.Value })
            };
        }

        public async Task<IEnumerable<Claim>> ObtemClaims(UserResponse User)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, User.Nome));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, User.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, User.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            if (User.PerfilUser == TipoUser.Administrador) claims.Add(new Claim(ClaimTypes.Role, "AdminRole"));
            if (User.PerfilUser == TipoUser.Jogador) claims.Add(new Claim(ClaimTypes.Role, "JogadorRole"));
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
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(tokenDescriptor);
        }
    }
}
