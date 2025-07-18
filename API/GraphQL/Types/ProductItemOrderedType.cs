using API.Entities.OrderAggregate;

namespace API.GraphQL.Types;

public class ProductItemOrderedType : ObjectType<ProductItemOrdered>
{
    protected override void Configure(IObjectTypeDescriptor<ProductItemOrdered> descriptor)
    {
        descriptor.Field(pio => pio.ProductId).Type<NonNullType<IntType>>();
        descriptor.Field(pio => pio.Name).Type<NonNullType<StringType>>();
        descriptor.Field(pio => pio.PictureUrl).Type<StringType>();
    }
}
