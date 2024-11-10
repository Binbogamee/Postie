using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Postie.Configurations;
using Postie.DAL;
using Postie.Interfaces;
using Postie.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.UseCors(x =>
//{
//    x.WithHeaders().AllowAnyHeader();
//    x.WithOrigins("http://localhost:3000");
//    x.WithMethods().AllowAnyMethod();
//});

app.Run();
