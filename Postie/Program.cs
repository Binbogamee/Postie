using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Postie.DAL;
using Postie.Infrastructure;
using Postie.Interfaces;
using Postie.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true);

builder.Services.Configure<ServicesRoutes>(builder.Configuration.GetSection(nameof(ServicesRoutes)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.DocumentFilter<SwaggerFilter>("controllerPost");
});

builder.Services.AddDbContext<PostieDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PostieDbContext)));
    });

builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddHostedService<LifetimeService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
