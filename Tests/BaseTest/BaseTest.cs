using AccountService.Repositories;
using AuthHelper;
using AuthService.Jwt;
using AuthService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postie.DAL;
using Postie.Interfaces;
using PostService.Repositories;

namespace BaseTest
{
    [TestClass]
    public class BaseTest
    {
        private readonly ServiceProvider _serviceProvider;

        public readonly IAccountService AccountServiceInstance;
        public readonly IAuthService AuthServiceInstance;
        public readonly IPostService PostServiceInstance;

        public const string User_Password = "IvanIvan1";
        public const string User_Name = "Ivan";
        public const string User_Email = "Ivanov@ivan.com";
        public BaseTest()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationManager();
            configuration.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appConfig.json"), true, true)
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "auth.json"), true, true)
                .AddEnvironmentVariables();

            var connectionString = configuration.GetConnectionString(nameof(PostieDbContext));
            if (connectionString != null)
            {
                var databaseSubString = connectionString.Substring(connectionString.IndexOf("Database="));
                var separatorIndex = databaseSubString.IndexOf(';');
                var databaseName = databaseSubString.Substring(0, separatorIndex);
                connectionString = connectionString.Replace(databaseName, "Database=testPostiedb");
            }

            services.AddDbContext<PostieDbContext>(
                options =>
                {
                    options.UseNpgsql(connectionString);
                });

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountService, AccountService.Services.AccountService>();

            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IPostService, PostService.InternalServices.PostService>();


            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<IJwtProvider, JwtProvider>();
            services.AddTransient<IAuthService, AuthService.Services.AuthService>();
            _serviceProvider = services.BuildServiceProvider();

            AccountServiceInstance = _serviceProvider.GetService<IAccountService>();
            AuthServiceInstance = _serviceProvider.GetService<IAuthService>();
            PostServiceInstance = _serviceProvider.GetService<IPostService>();

        }

        [TestInitialize]
        public void TestInitialize()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PostieDbContext>();

                if (!context.Database.CanConnect())
                {
                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                }
                else
                {
                    ClearDatabase(context);
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PostieDbContext>();
                ClearDatabase(context);
            }
        }

        private void ClearDatabase(DbContext context)
        {
            var tables = context.Model.GetEntityTypes()
                                            .Select(t => t.GetTableName())
                                            .ToList();

            foreach (var table in tables)
            {
                var command = $"TRUNCATE TABLE \"{table}\" CASCADE";
                context.Database.ExecuteSqlRaw(command);
            }
        }
    }
}