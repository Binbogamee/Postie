using Confluent.Kafka;
using Newtonsoft.Json;
using NLog;
using Postie.Dtos;
using Postie.Models;

namespace LoggingService.InternalServices
{
    public class BaseLoggerService : BackgroundService
    {
        private readonly Logger _logger;
        private readonly string _topic;
        public IConsumer<Ignore, string> Consumer {  get; }

        public BaseLoggerService(IConfiguration configuration, KafkaTopic topic)
        {
            _logger = LogManager.GetCurrentClassLogger();
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

        public async Task Test()
        {
            Console.WriteLine("Audit service");
            await Task.Delay(TimeSpan.FromSeconds(3));
        }

        public void ProcessKafkaMessage(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = Consumer.Consume(stoppingToken);

                var message = JsonConvert.DeserializeObject<LogDto>(consumeResult.Message.Value);

                _logger.Log(message?.LogLevel, $"{message?.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing Kafka message: {ex.Message}");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Остановка Logging service...");
            await base.StopAsync(stoppingToken);
        }
    }
}
