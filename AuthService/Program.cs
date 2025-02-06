using AuthHelper;
using AuthService.Jwt;
using AuthService.Repositories;
using Microsoft.EntityFrameworkCore;
using Postie.DAL;
using Postie.Infrastructure;
using Postie.Interfaces;
using Shared.KafkaLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true)
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "auth.json"), true, true)
    .AddEnvironmentVariables();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostieDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PostieDbContext)));
    });

ExternalConfigSettings.Initialize(builder.Configuration);

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddHostedService<LifetimeService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
