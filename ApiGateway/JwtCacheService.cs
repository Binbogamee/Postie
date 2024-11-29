using StackExchange.Redis;

namespace ApiGateway
{
    public class JwtCacheService
    {
        private readonly IDatabase _db;

        public JwtCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task AddInvalidTokenAsync(string key, string value, TimeSpan expiration)
        {
            await _db.StringSetAsync(key, value, expiration);
        }

        public async Task<bool> IsValidTokenAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty;
        }
    }
}
