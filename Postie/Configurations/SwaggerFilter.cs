using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Postie.Configurations
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
            // Находим пути, которые не соответствуют заданному префиксу, чтобы их убрать
            var pathsToRemove = swaggerDoc.Paths
                .Where(path => !path.Key.StartsWith("/" + _prefix))
                .Select(p => p.Key)
                .ToList();

            // Удаляем неподходящие пути
            foreach (var path in pathsToRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}
