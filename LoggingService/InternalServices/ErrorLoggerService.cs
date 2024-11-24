using Shared.KafkaLogging;

namespace LoggingService.InternalServices
{
    public class ErrorLoggerService : BaseLoggerService
    {
        public ErrorLoggerService(IConfiguration configuration)
            : base(configuration, KafkaTopic.Errors)
        {
        }
    }
}
