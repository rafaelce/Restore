using API.Entities;
using API.Entities.OrderAggregate;
using API.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.GraphQL.Queries;

public class Query
{
    // Método com ApplicationContext (usando [Service])
        public int ProductCount([Service] ApplicationContext context) // Removido "Get"
        {
            try
            {
                return context.Products.Count();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        // Product Queries (usando [Service])
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> Products([Service] ApplicationContext context) // Removido "Get"
        {
            return context.Products;
        }

        public Product? Product(int id, [Service] ApplicationContext context) // Removido "Get"
        {
            return context.Products.Find(id);
        }

        [UsePaging]
        public IQueryable<Product> ProductsByBrand(string brand, [Service] ApplicationContext context) // Removido "Get"
        {
            return context.Products.Where(p => p.Brand == brand);
        }

        // User Queries (usando [Service])
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> Users([Service] ApplicationContext context) // Removido "Get"
        {
            return context.Users;
        }

        public User? User(string id, [Service] ApplicationContext context) // Removido "Get"
        {
            return context.Users.Find(id);
        }

        // Order Queries (usando [Service])
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Order> Orders([Service] ApplicationContext context) // Removido "Get"
        {
            return context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .Include(o => o.PaymentSummary);
        }

        public Order? Order(int id, [Service] ApplicationContext context) // Removido "Get"
        {
            return context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .Include(o => o.PaymentSummary)
                .FirstOrDefault(o => o.Id == id);
        }

        [UsePaging]
        public IQueryable<Order> OrdersByEmail(string email, [Service] ApplicationContext context) // Removido "Get"
        {
            return context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .Include(o => o.PaymentSummary)
                .Where(o => o.BuyerEmail == email);
        }
}
