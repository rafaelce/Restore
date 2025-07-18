using API.Entities.OrderAggregate;

namespace API.GraphQL.Types;

public class OrderType : ObjectType<Order>
{
    protected override void Configure(IObjectTypeDescriptor<Order> descriptor)
    {
        descriptor.Field(o => o.Id).Type<NonNullType<IntType>>();
        descriptor.Field(o => o.BuyerEmail).Type<NonNullType<StringType>>();
        descriptor.Field(o => o.ShippingAddress).Type<NonNullType<ShippingAddressType>>();
        descriptor.Field(o => o.OrderDate).Type<NonNullType<DateTimeType>>();
        descriptor.Field(o => o.OrderItems).Type<ListType<OrderItemType>>();
        descriptor.Field(o => o.Subtotal).Type<NonNullType<LongType>>();
        descriptor.Field(o => o.DeliveryFee).Type<NonNullType<LongType>>();
        descriptor.Field(o => o.Discout).Type<NonNullType<LongType>>();
        descriptor.Field(o => o.PaymentIntentId).Type<NonNullType<StringType>>();
        descriptor.Field(o => o.OrderStatus).Type<NonNullType<EnumType<OrderStatus>>>();
        descriptor.Field(o => o.PaymentSummary).Type<NonNullType<PaymentSummaryType>>();
    }
}
