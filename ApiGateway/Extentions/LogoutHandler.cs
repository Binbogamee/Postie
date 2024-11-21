using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace ApiGateway.Extentions
{
    public class LogoutHandler : DelegatingHandler
    {
        private readonly JwtTokenManager _jwtTokenManager = JwtTokenManager.Instance;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authstring = request.Headers.Authorization?.Parameter;

            if (String.IsNullOrEmpty(authstring))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(authstring);
            var tokenS = jsonToken as JwtSecurityToken;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp");

            var expDate = _jwtTokenManager.GetJwtExp(exp);

            if (expDate == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            _jwtTokenManager.AddInvalidToken(authstring, (DateTime)expDate);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
