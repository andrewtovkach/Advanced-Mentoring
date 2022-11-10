using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Categories.Commands.DeleteCategory;
using CatalogService.Application.Categories.Commands.UpdateCategory;
using CatalogService.Application.Items.Commands.CreateItem;
using CatalogService.Application.Items.Commands.DeleteItem;
using CatalogService.Application.Items.Commands.UpdateItem;
using CatalogService.GraphQLAPI.GraphQL.GraphQLTypes;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLQueries
{
    public class AppMutation : ObjectGraphType
    {
        public AppMutation(ISender mediator)
        {
            FieldAsync<IntGraphType>(
                "createCategory",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var createCategoryCommand = context.GetArgument<CreateCategoryCommand>("category");
                    return await mediator.Send(createCategoryCommand);
                }
            );

            FieldAsync<IntGraphType>(
                "createItem",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ItemInputType>> { Name = "item" }),
                resolve: async context =>
                {
                    var createItemCommand = context.GetArgument<CreateItemCommand>("item");
                    return await mediator.Send(createItemCommand);
                }
            );

            FieldAsync<StringGraphType>(
                "updateCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "categoryId" }),
                resolve: async context =>
                {
                    var updateCategoryCommand = context.GetArgument<UpdateCategoryCommand>("category");
                    var categoryId = context.GetArgument<int>("categoryId");

                    if (categoryId < 0)
                    {
                        context.Errors.Add(new ExecutionError("Category Id should be a positive number."));
                        return null;
                    }

                    updateCategoryCommand.Id = categoryId;

                    await mediator.Send(updateCategoryCommand);

                    return $"The category with the id: {categoryId} has been successfully updated.";
                }
            );

            FieldAsync<StringGraphType>(
                "updateItem",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ItemInputType>> { Name = "item" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "itemId" }),
                resolve: async context =>
                {
                    var updateItemCommand = context.GetArgument<UpdateItemCommand>("item");
                    var itemId = context.GetArgument<int>("itemId");

                    if (itemId < 0)
                    {
                        context.Errors.Add(new ExecutionError("Item Id should be a positive number."));
                        return null;
                    }

                    updateItemCommand.Id = itemId;

                    await mediator.Send(updateItemCommand);

                    return $"The item with the id: {itemId} has been successfully updated.";
                }
            );

            FieldAsync<StringGraphType>(
                "deleteCategory",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "categoryId" }),
                resolve: async context =>
                {
                    var categoryId = context.GetArgument<int>("categoryId");

                    if (categoryId < 0)
                    {
                        context.Errors.Add(new ExecutionError("Category Id should be a positive number."));
                        return null;
                    }

                    await mediator.Send(new DeleteCategoryCommand { Id = categoryId });

                    return $"The category with the id: {categoryId} has been successfully deleted.";
                }
            );

            FieldAsync<StringGraphType>(
                "deleteItem",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "itemId" }),
                resolve: async context =>
                {
                    var itemId = context.GetArgument<int>("itemId");

                    if (itemId < 0)
                    {
                        context.Errors.Add(new ExecutionError("Item Id should be a positive number."));
                        return null;
                    }

                    await mediator.Send(new DeleteItemCommand { Id = itemId });

                    return $"The item with the id: {itemId} has been successfully deleted.";
                }
            );
        }
    }
}
