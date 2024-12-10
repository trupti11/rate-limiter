using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.API.Middleware;

[ExcludeFromCodeCoverage]
public class SwaggerCallSourceFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Required = true,
            Name = "X-Client",
            In = ParameterLocation.Header,
            Content = new ConcurrentDictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new()
            }
        });
    }
}
