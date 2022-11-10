namespace CatalogService.Application.Common.Models
{
    public class Link
    {
        public string Href { get; set; }

        public string Rel { get; set; }

        public string Method { get; set; }

        public Link(string href, string method, string rel = "self")
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
