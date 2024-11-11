using NLog;

namespace Postie.Dtos
{
    public class LogDto
    {
        public LogArea Area { get; set; }
        public NLog.LogLevel LogLevel { get; set; }
        public string Message { get; set; }
    }

    public enum LogArea
    {
        Core,
        Heartbeat
    }
}
