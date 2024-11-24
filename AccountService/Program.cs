using AccountService.Repositories;
using Microsoft.EntityFrameworkCore;
using Postie.DAL;
using Postie.Interfaces;
using Shared.KafkaLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostieDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PostieDbContext)));
    });

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddScoped<IAccountService, AccountService.Services.AccountService>();
builder.Services.AddHostedService<LifetimeService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
