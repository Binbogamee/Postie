using Postie.Models;

namespace LoggingService.InternalServices
{
    public class HeartbeatLoggerService : BaseLoggerService
    {
        public HeartbeatLoggerService(IConfiguration configuration)
            : base(configuration, KafkaTopic.Heartbeat)
        {
        }
    }
}
