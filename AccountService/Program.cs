using AccountService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Postie.Configurations;
using Postie.DAL;
using Postie.Interfaces;
using Postie.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), true, true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.DocumentFilter<SwaggerFilter>("Account");
});

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

app.UseAuthorization();

app.MapControllers();

app.Run();
