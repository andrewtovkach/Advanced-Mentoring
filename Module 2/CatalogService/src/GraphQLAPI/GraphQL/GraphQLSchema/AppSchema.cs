using System;
using CatalogService.GraphQLAPI.GraphQL.GraphQLQueries;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.GraphQLAPI.GraphQL.GraphQLSchema
{
    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<AppQuery>();
            Mutation = provider.GetRequiredService<AppMutation>();
        }
    }
}
