using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway
{
    public sealed class JwtTokenManager
    {
        private static readonly Lazy<JwtTokenManager> _instance = new Lazy<JwtTokenManager>(() => new JwtTokenManager());

        public static JwtTokenManager Instance = _instance.Value;

        private readonly string _authType = "Bearer";
        private readonly DateTime _startUnixDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public string GetRequestToken(StringValues authstrings)
        {
            if (authstrings.Count != 1)
            {
                return string.Empty;
            }

            try
            {
                var auth = authstrings[0];
                var index = auth.LastIndexOf(_authType);
                return auth.Substring(index + 1 + _authType.Length).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        public DateTime? GetJwtExp(string authstring)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(authstring);
            var tokenS = jsonToken as JwtSecurityToken;
            var claim = tokenS.Claims.First(claim => claim.Type == "exp");

            if (claim != null)
            {
                var expunix = claim.Value;
                double expDate;
                if (double.TryParse(expunix, out expDate))
                {
                    return _startUnixDate.AddSeconds(expDate);
                }
            }

            return null;
        }

        public Claim? GetJwtClaimAccountId(string authstring)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(authstring);
            var tokenS = jsonToken as JwtSecurityToken;
            return tokenS.Claims.First(claim => claim.Type == "requesterId");

        }
    }
}
