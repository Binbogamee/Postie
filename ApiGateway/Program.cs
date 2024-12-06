using ApiGateway;
using ApiGateway.Extentions;
using AuthHelper;
using Ocelot.DependencyInjection;
using Shared.KafkaLogging;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile("auth.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appConfig.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetSection("Redis:ConnectionString").Value));

ExternalConfigSettings.Initialize(builder.Configuration);

builder.Services.ConfigureRoutesPlaceholders(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddHostedService<LifetimeService>();
builder.Services.AddSingleton<JwtCacheService>();
builder.Services.AddOcelot().AddDelegatingHandler<LogoutHandler>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

OcelotMiddlewareExtentions.Configure(app.Services);
await app.AddOcelotConfiguration();

app.Run();
