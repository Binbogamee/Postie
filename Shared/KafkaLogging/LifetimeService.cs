using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System.Reflection;

namespace Shared.KafkaLogging
{
    public class LifetimeService : BackgroundService
    {
        private readonly Logger _logger;
        private readonly string _projectName;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILoggingProducerService _loggingProducerService;
        private readonly IServiceProvider _serviceProvider;

        public LifetimeService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        {
            _projectName = Assembly.GetEntryAssembly().GetName().Name ?? string.Empty;
            _logger = LogManager.GetLogger("HeartbeatLogger");
            _appLifetime = appLifetime;
            _serviceProvider = serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                _loggingProducerService = scope.ServiceProvider.GetRequiredService<ILoggingProducerService>();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                _appLifetime.ApplicationStarted.Register(OnStarted);
            });

            Task.Run(() =>
            {
                _appLifetime.ApplicationStopped.Register(OnStopped);
            });

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _loggingProducerService.SendLogMessage(NLog.LogLevel.Info, $"{_projectName} started", LogArea.Heartbeat);
        }

        private void OnStopped()
        {
            _loggingProducerService.SendLogMessage(NLog.LogLevel.Info, $"{_projectName} stopped", LogArea.Heartbeat);
        }
    }
}
