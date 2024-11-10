using Confluent.Kafka;
using Newtonsoft.Json;
using Postie.Dtos;
using Postie.Interfaces;
using Postie.Models;

namespace Postie.Services
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

        public async void SendLogMessage(NLog.LogLevel level, string message)
        {
            try
            {
                var topic = string.Empty;
                var request = CreateLogMessage(level, message, out topic);
                if (String.IsNullOrEmpty(topic))
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

        private LogDto CreateLogMessage(NLog.LogLevel level, string message, out string topic)
        {
            var request = new LogDto()
            {
                LogLevel = level,
                Message = message
            };

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

            return request;
        } 
    }
}
