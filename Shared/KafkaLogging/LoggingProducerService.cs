using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Shared.KafkaLogging
{
    public class LoggingProducerService : ILoggingProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<Null, string> _producer;

        public LoggingProducerService(IConfiguration configuration)
        {
            _configuration = configuration;

            var producerconfig = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                SecurityProtocol = SecurityProtocol.Plaintext,
            };

            _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
        }

        public async void SendLogMessage(NLog.LogLevel level, string message, LogArea area = LogArea.Core)
        {
            try
            {
                var topic = string.Empty;
                var request = CreateLogMessage(area, level, message, out topic);
                if (string.IsNullOrEmpty(topic))
                {
                    return;
                }

                using (_producer)
                {
                    var kafkaMessage = new Message<Null, string> { Value = JsonConvert.SerializeObject(request) };
                    await _producer.ProduceAsync(topic, kafkaMessage);
                }
            }
            catch (Exception ex)
            {
                //do what?
            }
        }

        private LogDto CreateLogMessage(LogArea area, NLog.LogLevel level, string message, out string topic)
        {
            var request = new LogDto()
            {
                Area = area,
                LogLevel = level,
                Message = message
            };

            if (area == LogArea.Heartbeat)
            {
                topic = KafkaTopic.Heartbeat.ToString();
            }
            else
            {
                switch (level.Ordinal)
                {
                    case 4:
                    case 5:
                        topic = KafkaTopic.Errors.ToString();
                        break;
                    default:

                        topic = KafkaTopic.Audit.ToString();
                        break;
                }
            }

            return request;
        }
    }
}
