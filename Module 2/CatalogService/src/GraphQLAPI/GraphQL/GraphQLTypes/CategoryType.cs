using System.Linq;
using CatalogService.Application.Categories.Queries.GetCategories;
using CatalogService.Application.Items.Queries.GetItemsByCategoryId;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLTypes
{
    public class CategoryType : ObjectGraphType<CategoryDto>
    {
        public CategoryType(ISender mediator, IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the category object.");
            Field(x => x.Name, nullable: true).Description("Name property from the category object.");
            Field(x => x.Image, nullable: true).Description("Image property from the category object.");
            Field(x => x.ParentCategory, nullable: true).Description("Parent category property from the category object.");
            Field<ListGraphType<ItemType>>("items",
                resolve: context =>
                {
                    var loader = dataLoaderContextAccessor.Context.GetOrAddCollectionBatchLoader<int, ItemDto>("GetItemByCategoryId",
                        async (ids) => await mediator.Send(new GetItemsByCategoryIdQuery { CategoryIds = ids.ToList() }));

                    return loader.LoadAsync(context.Source.Id);
                });
        }
    }
}
