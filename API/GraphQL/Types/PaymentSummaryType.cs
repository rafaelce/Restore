using API.Entities.OrderAggregate;

namespace API.GraphQL.Types;

public class PaymentSummaryType : ObjectType<PaymentSummary>
{
    protected override void Configure(IObjectTypeDescriptor<PaymentSummary> descriptor)
    {
        descriptor.Field(ps => ps.Last4).Type<NonNullType<IntType>>();
        descriptor.Field(ps => ps.Brand).Type<NonNullType<StringType>>();
        descriptor.Field(ps => ps.ExpMonth).Type<NonNullType<IntType>>();
        descriptor.Field(ps => ps.ExpYear).Type<NonNullType<IntType>>();
    }
}
