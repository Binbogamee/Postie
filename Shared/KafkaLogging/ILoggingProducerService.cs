namespace Shared.KafkaLogging
{
    public interface ILoggingProducerService
    {
        public void SendLogMessage(NLog.LogLevel level, string message, LogArea area = LogArea.Core);
    }
}
