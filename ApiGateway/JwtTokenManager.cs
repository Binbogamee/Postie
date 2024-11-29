using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway
{
    public sealed class JwtTokenManager
    {
        private static readonly Lazy<JwtTokenManager> _instance = new Lazy<JwtTokenManager>(() => new JwtTokenManager());

        public static JwtTokenManager Instance = _instance.Value;
        private JwtTokenManager()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            JwtCleanStart();
        }

        private ConcurrentDictionary<string, DateTime> _invalidTokens = new ConcurrentDictionary<string, DateTime>();
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _jwtCleaner;
        private readonly int _cleanerInterval = 5 * 60 * 1000; // 5 min
        private readonly string _authType = "Bearer";
        private readonly DateTime _startUnixDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public bool IsValidToken(string token)
        {
            if (_invalidTokens.TryGetValue(token, out _))
            {
                return false;
            }

            return true;
        }

        public void AddInvalidToken(string token, DateTime expiredDate)
        {
            _invalidTokens[token] = expiredDate;
        }

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

        public DateTime? GetJwtExp(Claim? claim)
        {
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

        private void JwtCleanStart()
        {
            _jwtCleaner = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var now = DateTime.UtcNow;

                        var keysToRemove = _invalidTokens.Where(x => x.Value < now).Select(x => x.Key).ToList();

                        foreach (var key in keysToRemove)
                        {
                            _invalidTokens.TryRemove(key, out _);
                        }

                        await Task.Delay(_cleanerInterval);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error clearing JWT token dictionary: {ex.Message}");
                    }
                }
            });
        }

        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _jwtCleaner?.Wait();
        }
    }
}
