using System.Collections.Generic;

namespace CatalogService.Application.Common.Models
{
    public class LinkCollectionWrapper<T> : LinkResourceBase
    {
        public List<T> Value { get; set; } = new List<T>();

        public LinkCollectionWrapper(List<T> value, List<Link> links)
        {
            Value = value;
            Links = links;
        }
    }
}
