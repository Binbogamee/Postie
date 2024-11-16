using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Postie.Infrastructure
{
    public class SwaggerFilter : IDocumentFilter
    {
        private readonly string _prefix;

        public SwaggerFilter(string prefix)
        {
            _prefix = prefix;
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathsToRemove = swaggerDoc.Paths
                .Where(path => !path.Key.StartsWith("/" + _prefix))
                .Select(p => p.Key)
                .ToList();

            foreach (var path in pathsToRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}
