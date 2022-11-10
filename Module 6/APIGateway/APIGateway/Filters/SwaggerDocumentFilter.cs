using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace APIGateway.Filters
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        readonly string[] _ignoredPaths = { "/configuration", "/outputcache/{region}" };

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var ignorePath in _ignoredPaths)
            {
                swaggerDoc.Paths.Remove(ignorePath);
            }
        }
    }
}
