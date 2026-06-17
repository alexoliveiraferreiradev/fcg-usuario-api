namespace Fcg.Usuarios.Application.Features.Token
{
    public class TokenResult
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public IEnumerable<ClaimResponse> Claims { get; set; }
    }
}
