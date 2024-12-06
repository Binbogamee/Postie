using NLog;
using System.Reflection;

namespace LoggingService.InternalServices
{
    public class LifetimeLoggerService : BackgroundService
    {
        protected readonly Logger _logger;
        protected readonly string _projectName;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly string _ipAddress = string.Empty;
        private readonly string _kafkaServiceAddress = string.Empty;
        private readonly string _logConfigTemplate = "{0} configuration settings:\n" +
            "IP address: {1};\n" +
            "Kafka service: {2}.";

        public LifetimeLoggerService(IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _ipAddress = configuration["ASPNETCORE_URLS"] ?? string.Empty;
            _kafkaServiceAddress = configuration["Kafka:BootstrapServers"] ?? string.Empty;

            _projectName = Assembly.GetEntryAssembly().GetName().Name ?? string.Empty;
            _logger = LogManager.GetLogger("HeartbeatLogger");
            _appLifetime = appLifetime;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => {
                _appLifetime.ApplicationStarted.Register(OnStarted);
            });

            Task.Run(() => {
                _appLifetime.ApplicationStopped.Register(OnStopped);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.Info($"{_projectName} started");
            _logger.Info(String.Format(_logConfigTemplate, _projectName, _ipAddress, _kafkaServiceAddress));
        }

        private void OnStopped()
        {
            _logger.Info($"{_projectName} stopped");
        }
    }
}
