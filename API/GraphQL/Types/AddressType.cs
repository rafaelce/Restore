using API.Entities;

namespace API.GraphQL.Types;

public class AddressType : ObjectType<Address>
{
    protected override void Configure(IObjectTypeDescriptor<Address> descriptor)
    {
        descriptor.Field(a => a.Id).Type<NonNullType<IntType>>();
        descriptor.Field(a => a.Name).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Line1).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Line2).Type<StringType>();
        descriptor.Field(a => a.City).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.State).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.PostalCode).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Country).Type<NonNullType<StringType>>();
    }
}
