using Confluent.Kafka;
using Newtonsoft.Json;
using NLog;
using Postie.Dtos;
using Postie.Models;

namespace LoggingService.InternalServices
{
    public class BaseLoggerService : BackgroundService
    {
        private readonly Logger _coreLogger;
        private readonly Logger _heartbeatLogger;
        private readonly string _topic;
        public IConsumer<Ignore, string> Consumer {  get; }

        public BaseLoggerService(IConfiguration configuration, KafkaTopic topic)
        {
            _coreLogger = LogManager.GetLogger("CoreLogger");
            _heartbeatLogger = LogManager.GetLogger("HeartbeatLogger");
            _topic = topic.ToString();

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = _topic + "-group",
                ApiVersionRequest = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SecurityProtocol = SecurityProtocol.Plaintext
            };

            Consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            Consumer.Subscribe(_topic);

            _heartbeatLogger.Info($"Consume \"{_topic}\" Kafka topic");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ProcessKafkaMessage(stoppingToken);

                    Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }, stoppingToken);

            Consumer.Close();
        }

        public void ProcessKafkaMessage(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = Consumer.Consume(stoppingToken);

                var message = JsonConvert.DeserializeObject<LogDto>(consumeResult.Message.Value);

                if (message == null)
                {
                    return;
                }

                if (message.Area == LogArea.Core)
                {
                    _coreLogger.Log(message.LogLevel, $"{message.Message}");
                }
                else
                {
                    _heartbeatLogger.Log(message.LogLevel, $"{message.Message}");
                }
            }
            catch (Exception ex)
            {
                _coreLogger.Error($"Error processing Kafka message: {ex.Message}");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
