using System.Linq;
using CatalogService.Application.Categories.Queries.GetCategories;
using CatalogService.Application.Categories.Queries.GetCategoriesByIds;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLTypes
{
    public class ItemType : ObjectGraphType<ItemDto>
    {
        public ItemType(ISender mediator, IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the item object.");
            Field(x => x.Name, nullable: true).Description("Name property from the item object.");
            Field(x => x.Description, nullable: true).Description("Description property from the item object.");
            Field(x => x.Image, nullable: true).Description("Image property from the item object.");
            Field(x => x.Price).Description("Price category property from the item object.");
            Field(x => x.Amount).Description("Amount category property from the item object.");
            Field(x => x.CategoryId).Description("CategoryId property from the item object.");
            Field<CategoryType>("category",
                resolve: context =>
                {
                    var loader = dataLoaderContextAccessor.Context.GetOrAddBatchLoader<int, CategoryDto>("GetCategoriesByIds",
                        async (ids) => await mediator.Send(new GetCategoriesByIdsQuery { Ids = ids.ToList() }));

                    return loader.LoadAsync(context.Source.CategoryId);
                });
        }
    }
}
