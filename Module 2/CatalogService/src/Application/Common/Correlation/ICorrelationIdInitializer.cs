namespace CatalogService.Application.Common.Correlation
{
    public interface ICorrelationIdInitializer
    {
        string CorrelationId { get; }
    }
}
