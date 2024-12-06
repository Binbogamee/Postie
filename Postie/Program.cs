using Microsoft.EntityFrameworkCore;
using Postie.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<PostieDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PostieDbContext)));
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
