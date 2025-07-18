using API.DTO;
using API.DTO.GraphQL;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.GraphQL.Mutations;

public class Mutation
{
    // Product Mutations
    public async Task<Product> CreateProduct(CreateProductGraphQLDto input, [Service] ApplicationContext context)
    {
        var product = new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            PictureUrl = "", // Será preenchido pelo ImageService
            Type = input.Type,
            Brand = input.Brand,
            QuantityInStock = input.QuantityInStock
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> UpdateProduct(int id, UpdateProductGraphQLDto input, [Service] ApplicationContext context)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return null;

        product.Name = input.Name;
        product.Description = input.Description;
        product.Price = input.Price;
        product.Type = input.Type;
        product.Brand = input.Brand;
        product.QuantityInStock = input.QuantityInStock;

        await context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(int id, [Service] ApplicationContext context)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return false;

        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return true;
    }

    // Order Mutations
    public async Task<Order?> CreateOrder(CreateOrderDto input, [Service] ApplicationContext context)
    {
        var order = new Order
        {
            BuyerEmail = "test@example.com", // Será obtido do usuário autenticado
            ShippingAddress = input.ShippingAddress,
            OrderDate = DateTime.UtcNow,
            OrderStatus = OrderStatus.Pending,
            PaymentIntentId = "pi_test_" + Guid.NewGuid().ToString("N"),
            PaymentSummary = input.PaymentSummary
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> UpdateOrderStatus(int id, OrderStatus status, [Service] ApplicationContext context)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
            return null;

        order.OrderStatus = status;
        await context.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> AddOrderItem(int orderId, int productId, int quantity, [Service] ApplicationContext context)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return null;

        var product = await context.Products.FindAsync(productId);
        if (product == null)
            return null;

        var orderItem = new OrderItem
        {
            ItemOrdered = new ProductItemOrdered
            {
                ProductId = product.Id,
                Name = product.Name,
                PictureUrl = product.PictureUrl
            },
            Price = product.Price,
            Quantity = quantity
        };

        order.OrderItems.Add(orderItem);
        await context.SaveChangesAsync();

        return order;
    }
}