using AccountService.Repositories;
using Microsoft.EntityFrameworkCore;
using Postie.DAL;
using Postie.Infrastructure;
using Postie.Interfaces;
using Shared.KafkaLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true)
    .AddEnvironmentVariables();

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
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddScoped<IAccountService, AccountService.Services.AccountService>();
builder.Services.AddHostedService<LifetimeService>();


var app = builder.Build();

app.UseMiddleware<RequesterIdMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
