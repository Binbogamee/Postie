namespace Postie.Interfaces
{
    public interface ILoggingProducerService
    {
        public void SendLogMessage(NLog.LogLevel level, string message);
    }
}
