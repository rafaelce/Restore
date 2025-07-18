using API.Entities.OrderAggregate;

namespace API.GraphQL.Types;

public class OrderItemType : ObjectType<OrderItem>
{
    protected override void Configure(IObjectTypeDescriptor<OrderItem> descriptor)
    {
        descriptor.Field(oi => oi.Id).Type<NonNullType<IdType>>();
        descriptor.Field(oi => oi.ItemOrdered).Type<NonNullType<ProductItemOrderedType>>();
        descriptor.Field(oi => oi.Price).Type<NonNullType<DecimalType>>();
        descriptor.Field(oi => oi.Quantity).Type<NonNullType<IntType>>();
    }
}
