using Ocelot.Middleware;
using System.Net;

namespace ApiGateway.Extentions
{
    public static class OcelotMiddlewareExtentions
    {
        private static JwtCacheService _jwtCacheService;
        public static void Configure(IServiceProvider serviceProvider)
        {
            var service = serviceProvider?.GetRequiredService<JwtCacheService>();
            if (service == null)
            {
                throw new InvalidOperationException("JwtCacheService is not configured.");
            }

            _jwtCacheService = service;
        }

        public static async Task AddOcelotConfiguration(this WebApplication app)
        {
            var jwtManager = JwtTokenManager.Instance;

            var configuration = new OcelotPipelineConfiguration
            {
                PreAuthenticationMiddleware = async (context, next) =>
                {
                    if (context.Request.Path == "/api/Login")
                    {
                        await next.Invoke();
                        return;
                    }
                    var authstring = jwtManager.GetRequestToken(context.Request.Headers.Authorization);
                    if (await _jwtCacheService.IsValidTokenAsync(authstring))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }
            };

            await app.UseMiddleware<RequestResponseMiddleware>().UseOcelot(configuration);
        }
    }
}
