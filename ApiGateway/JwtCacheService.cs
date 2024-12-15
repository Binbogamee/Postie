using Shared.KafkaLogging;
using StackExchange.Redis;

namespace ApiGateway
{
    public class JwtCacheService
    {
        private readonly IDatabase _db;

        public JwtCacheService(IConnectionMultiplexer redis, IServiceProvider serviceProvider)
        {
            if (redis != null)
            {
                _db = redis.GetDatabase();
            }
            else
            {
                ILoggingProducerService loggingProducerService;
                using (var scope = serviceProvider.CreateScope())
                {
                    loggingProducerService = scope.ServiceProvider.GetRequiredService<ILoggingProducerService>();
                }

                loggingProducerService.SendLogMessage(NLog.LogLevel.Error, "Redis is not connected.", LogArea.Heartbeat);
            }

        }

        public async Task AddInvalidTokenAsync(string key, string value, TimeSpan expiration)
        {
            if (_db != null)
            {
                await _db.StringSetAsync(key, value, expiration);
            }
        }

        public async Task<bool> IsValidTokenAsync(string key)
        {
            if (_db == null)
            {
                return false;
            }
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty;
        }
    }
}
