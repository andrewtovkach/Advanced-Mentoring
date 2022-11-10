using CorrelationId.Abstractions;

namespace CatalogService.Application.Common.Correlation
{
    public class CorrelationIdInitializer : ICorrelationIdInitializer
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationIdInitializer(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        public string CorrelationId => _correlationContextAccessor.CorrelationContext.CorrelationId;
    }
}
