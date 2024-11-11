using Postie.Dtos;

namespace Postie.Interfaces
{
    public interface ILoggingProducerService
    {
        public void SendLogMessage(NLog.LogLevel level, string message, LogArea area = LogArea.Core);
    }
}
