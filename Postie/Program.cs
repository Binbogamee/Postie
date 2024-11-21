using Microsoft.EntityFrameworkCore;
using Postie.DAL;
using Postie.Interfaces;
using Postie.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true);

builder.Services.AddDbContext<PostieDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PostieDbContext)));
    });

builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddHostedService<LifetimeService>();
var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
