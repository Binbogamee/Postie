using Ocelot.Configuration.File;

namespace ApiGateway.Extentions
{
    public static class FileConfigurationExtensions
    {
        public static void ConfigureRoutesPlaceholders(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.PostConfigure<FileConfiguration>(fileConfiguration =>
            {
                var globalHosts = configuration
                    .GetSection($"{nameof(FileConfiguration.GlobalConfiguration)}:Hosts")
                    .Get<GlobalHosts>();

                if (globalHosts == null)
                {
                    return;
                }

                foreach (var route in fileConfiguration.Routes)
                {
                    ConfigureRoute(route, globalHosts);
                }
            });
        }

        private static void ConfigureRoute(FileRoute route, GlobalHosts globalHosts)
        {
            foreach (var hostAndPort in route.DownstreamHostAndPorts)
            {
                var host = hostAndPort.Host;
                if (host.StartsWith("{") && host.EndsWith("}"))
                {
                    var placeHolder = host.TrimStart('{').TrimEnd('}');
                    if (globalHosts.TryGetValue(placeHolder, out var uri))
                    {
                        route.DownstreamScheme = uri.Scheme;
                        hostAndPort.Host = uri.Host;
                        hostAndPort.Port = uri.Port;
                    }
                }
            }
        }
    }

    public class GlobalHosts : Dictionary<string, Uri> { }
}
