using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace ApiGateway.Extentions
{
    public class LogoutHandler : DelegatingHandler
    {
        private readonly JwtTokenManager _jwtTokenManager = JwtTokenManager.Instance;
        private readonly JwtCacheService _jwtCacheService;

        public LogoutHandler(JwtCacheService jwtCacheService)
        {
            _jwtCacheService = jwtCacheService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authstring = request.Headers.Authorization?.Parameter;
            if (String.IsNullOrEmpty(authstring))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            var expDate = _jwtTokenManager.GetJwtExp(authstring);
            if (expDate == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            var expiration = (DateTime)expDate;
            await _jwtCacheService.AddInvalidTokenAsync(authstring, expiration.ToString(), (expiration - DateTime.UtcNow));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
