using NLog;

namespace Postie.Dtos
{
    public class LogDto
    {
        public NLog.LogLevel LogLevel { get; set; }
        public string Message { get; set; }
    }
}
