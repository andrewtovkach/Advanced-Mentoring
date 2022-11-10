using CatalogService.Application.Categories.Queries.GetCategories;
using CatalogService.Application.Items.Queries.GetItems;
using GraphQL.Types;
using CatalogService.GraphQLAPI.GraphQL.GraphQLTypes;
using MediatR;
using GraphQL;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLQueries
{
    public class AppQuery : ObjectGraphType
    {
        public AppQuery(ISender mediator)
        {
            FieldAsync<ListGraphType<CategoryType>>(
               "categories",
               resolve: async context => await mediator.Send(new GetCategoriesQuery())
            );

            FieldAsync<ListGraphType<ItemType>>(
               "items",
               arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "categoryId" },
                    new QueryArgument<IntGraphType> { Name = "pageNumber" },
                    new QueryArgument<IntGraphType> { Name = "pageSize" }),
               resolve: async context =>
               {
                   var categoryId = context.GetArgument<int>("categoryId");
                   var pageNumber = context.GetArgument("pageNumber", 1);
                   var pageSize = context.GetArgument("pageSize", 10);

                   if (categoryId < 0)
                   {
                       context.Errors.Add(new ExecutionError("Category Id should be a positive number."));
                       return null;
                   }

                   if (pageNumber < 0)
                   {
                       context.Errors.Add(new ExecutionError("Page number should be a positive number."));
                       return null;
                   }

                   if (pageSize < 0)
                   {
                       context.Errors.Add(new ExecutionError("Page size Id should be a positive number."));
                       return null;
                   }

                   var result = await mediator.Send(new GetItemsQuery
                   {
                       CategoryId = categoryId,
                       PageNumber = pageNumber,
                       PageSize = pageSize
                   });

                   return result.Items;
               }
            );
        }
    }
}
