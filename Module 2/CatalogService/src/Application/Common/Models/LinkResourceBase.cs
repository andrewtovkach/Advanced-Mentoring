using System.Collections.Generic;

namespace CatalogService.Application.Common.Models
{
    public abstract class LinkResourceBase
    {
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
