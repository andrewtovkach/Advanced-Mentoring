using GraphQL.Types;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLTypes
{
    public class CategoryInputType : InputObjectGraphType
    {
        public CategoryInputType()
        {
            Name = "categoryInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<StringGraphType>("image");
            Field<StringGraphType>("parentCategory");
        }
    }
}
