using Microsoft.Extensions.Configuration;

namespace Shared.KafkaLogging
{
    public sealed class ExternalConfigSettings
    {
        private static Lazy<ExternalConfigSettings> _instance;

        public string IpAddress { get; } = string.Empty;
        public string DbHost { get; } = string.Empty;
        public string DbPort { get; } = string.Empty;
        public string DbName { get; } = string.Empty;
        public string KafkaPath { get; } = string.Empty;
        public string RedisPath { get; } = string.Empty;
        public int JwtExpiresMin { get; }
        public bool JwtKeyExisted { get; }

        private ExternalConfigSettings(IConfiguration configuration)
        {
            IpAddress = configuration["ASPNETCORE_URLS"] ?? string.Empty;
            var dbconnectString = configuration["ConnectionStrings:PostieDbContext"];
            if (dbconnectString != null)
            {
                var properties = dbconnectString.Split(';');
                var values = new Dictionary<string, string>();
                foreach (var value in properties)
                {
                    var splited = value.Split('=');
                    if (splited.Count() != 2)
                    {
                        continue;
                    }

                    values.Add(splited[0], splited[1]);
                }

                DbHost = values["Host"];
                DbPort = values["Port"];
                DbName = values["Database"];
            }

            KafkaPath = configuration["Kafka:BootstrapServers"] ?? string.Empty;
            RedisPath = configuration["Redis:ConnectionString"] ?? string.Empty;

            var jwtexp = configuration["JwtOptions:ExpiratesMinutes"] ?? string.Empty;
            var min = 0;
            Int32.TryParse(jwtexp, out min);
            JwtExpiresMin = min;

            JwtKeyExisted = !String.IsNullOrEmpty(configuration["JwtOptions:ExpiratesMinutes"]);
        }

        public static void Initialize(IConfiguration configuration)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("Singleton already initialized.");
            }

            _instance = new Lazy<ExternalConfigSettings>(() => new ExternalConfigSettings(configuration));
        }

        public static ExternalConfigSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Singleton not initialized. Call Initialize() first.");
                }

                return _instance.Value;
            }
        }

        private readonly string _logConfigTemplate = " configuration settings:\n" +
            "IP address: {0};\n" +
            "Database host: {1};\n" +
            "Database port: {2};\n" +
            "Database name: {3};\n" +
            "Kafka service: {4};\n" +
            "Redis service: {5};\n" +
            "JWT token expiration time (min): {6};\n" +
            "JWT Secret key exists: {7}.";

        public string GetConfigString()
        {
            return string.Format(_logConfigTemplate, IpAddress, DbHost, DbPort, DbName,
                KafkaPath, RedisPath, JwtExpiresMin, JwtKeyExisted);
        }
    }
}
