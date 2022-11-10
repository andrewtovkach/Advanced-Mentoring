using GraphQL.Types;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLTypes
{
    public class ItemInputType : InputObjectGraphType
    {
        public ItemInputType()
        {
            Name = "itemInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<StringGraphType>("description");
            Field<StringGraphType>("image");
            Field<NonNullGraphType<FloatGraphType>>("price");
            Field<NonNullGraphType<IntGraphType>>("amount");
            Field<NonNullGraphType<IntGraphType>>("categoryId");
        }
    }
}
