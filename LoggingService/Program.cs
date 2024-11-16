using NLog;
using NLog.Web;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true);
    builder.Services.AddHostedService<LoggingService.InternalServices.ErrorLoggerService>();
    builder.Services.AddHostedService<LoggingService.InternalServices.AuditLoggerService>();
    builder.Services.AddHostedService<LoggingService.InternalServices.HeartbeatLoggerService>();
    builder.Services.AddHostedService<LoggingService.InternalServices.LifetimeLoggerService>();
    var app = builder.Build();

    app.Run();
}
finally
{
    LogManager.Shutdown();
}