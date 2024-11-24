using Shared.KafkaLogging;

namespace LoggingService.InternalServices
{
    public class AuditLoggerService : BaseLoggerService
    {
        public AuditLoggerService(IConfiguration configuration) 
            : base(configuration, KafkaTopic.Audit)
        {
        }
    }
}
