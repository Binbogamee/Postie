using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true);
builder.Services.AddHostedService<LoggingService.InternalServices.ErrorLoggerService>();
builder.Services.AddHostedService<LoggingService.InternalServices.AuditLoggerService>();
var app = builder.Build();

app.Run();
