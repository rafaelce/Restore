using API.GraphQL.Mutations;
using API.GraphQL.Queries;
using API.GraphQL.Types;

namespace API.Modules;

public static class GraphQLModule
{
    public static IServiceCollection AddGraphQLModule(this IServiceCollection services)
    {
       services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddType<ProductType>()
                .AddType<UserType>()
                .AddType<AddressType>()
                .AddType<OrderType>()
                .AddType<OrderItemType>()
                .AddType<ProductItemOrderedType>()
                .AddType<ShippingAddressType>()
                .AddType<PaymentSummaryType>()
                .AddFiltering()
                .AddSorting()
                .AddProjections()
                .AddInMemorySubscriptions()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

        return services;
    }
}