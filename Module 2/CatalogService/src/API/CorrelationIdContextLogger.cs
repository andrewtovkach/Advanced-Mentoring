using System;
using System.Threading.Tasks;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CatalogService.API
{
    public class CorrelationIdContextLogger
    {
        public CorrelationIdContextLogger(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        readonly RequestDelegate _next;

        public async Task InvokeAsync(HttpContext httpContext, ILogger<CorrelationIdContextLogger> logger, ICorrelationContextAccessor correlationContextAccessor)
        {
            if (Guid.TryParse(correlationContextAccessor.CorrelationContext.CorrelationId, out Guid correlationId))
            {
                using (logger.BeginScope(("CorrelationId", correlationId)))
                {
                    await _next(httpContext);
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
