using NLog;

namespace Shared.KafkaLogging
{
    public class LogDto
    {
        public LogArea Area { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
    }

    public enum LogArea
    {
        Core,
        Heartbeat
    }
}
