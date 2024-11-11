using NLog;
using System.Reflection;

namespace LoggingService.InternalServices
{
    public class LifetimeLoggerService : BackgroundService
    {
        protected readonly Logger _logger;
        protected readonly string _projectName;
        private readonly IHostApplicationLifetime _appLifetime;

        public LifetimeLoggerService(IHostApplicationLifetime appLifetime)
        {
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
        }

        private void OnStopped()
        {
            _logger.Info($"{_projectName} stopped");
        }
    }
}
