using API.Entities.OrderAggregate;

namespace API.GraphQL.Types;

public class ShippingAddressType : ObjectType<ShippingAddress>
{
    protected override void Configure(IObjectTypeDescriptor<ShippingAddress> descriptor)
    {
        descriptor.Field(sa => sa.Name).Type<NonNullType<StringType>>();
        descriptor.Field(sa => sa.Line1).Type<NonNullType<StringType>>();
        descriptor.Field(sa => sa.Line2).Type<StringType>();
        descriptor.Field(sa => sa.City).Type<NonNullType<StringType>>();
        descriptor.Field(sa => sa.State).Type<NonNullType<StringType>>();
        descriptor.Field(sa => sa.PostalCode).Type<NonNullType<StringType>>();
        descriptor.Field(sa => sa.Country).Type<NonNullType<StringType>>();
    }
}
