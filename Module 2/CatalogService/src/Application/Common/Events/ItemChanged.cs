using CatalogService.Application.Common.Correlation;

namespace CatalogService.Application.Common.Events
{
    public class ItemChanged : CorrelationMessage
    {
        public int Id { get; set; }

        public double Price { get; set; }

        public string Name { get; set; }
    }
}
