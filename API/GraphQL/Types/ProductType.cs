using API.Entities;

namespace API.GraphQL.Types;

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Field(p => p.Id).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Description).Type<StringType>();
        descriptor.Field(p => p.Price).Type<NonNullType<DecimalType>>();
        descriptor.Field(p => p.PictureUrl).Type<StringType>();
        descriptor.Field(p => p.Type).Type<StringType>();
        descriptor.Field(p => p.Brand).Type<StringType>();
        descriptor.Field(p => p.QuantityInStock).Type<IntType>();
    }
}
