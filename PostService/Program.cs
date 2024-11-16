using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Postie.Configurations;
using Postie.DAL;
using Postie.Interfaces;
using Postie.Services;
using PostService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.DocumentFilter<SwaggerFilter>("Post");
});

builder.Services.AddDbContext<PostieDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PostieDbContext)));
    });

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ILoggingProducerService, LoggingProducerService>();
builder.Services.AddScoped<IPostService, PostService.InternalServices.PostService>();
builder.Services.AddHostedService<LifetimeService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
