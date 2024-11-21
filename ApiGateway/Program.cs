using ApiGateway.Extentions;
using AuthHelper;
using Ocelot.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile("auth.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.ConfigureRoutesPlaceholders(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOcelot().AddDelegatingHandler<LogoutHandler>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
await app.AddOcelotConfiguration();

app.Run();
