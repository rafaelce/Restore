using API.Entities;
using API.Entities.OrderAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Infra.EntityFramework;

public class ApplicationContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public required DbSet<Product> Products { get; set; }
    public required DbSet<Basket> Baskets { get; set; }
    public required DbSet<Order> Orders { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
            .HasData(
                    new IdentityRole {
                        Id = "e069461a-10cf-4abf-9930-d070b2a7e40f",
                        Name = "Member",
                        NormalizedName = "MEMBER" },
                    new IdentityRole {
                        Id = "ed2e9149-fa53-484c-a93f-bd33f9e9fcf6",
                        Name = "Admin",
                        NormalizedName = "ADMIN" }
        );

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsyncWithoutTracker(CancellationToken cancellationToken = default)
   => base.SaveChangesAsync(cancellationToken);

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    private void ApplyUpdate()
    {
        var entities = ChangeTracker
                .Entries<dynamic>()
                .Where(t => t.State == EntityState.Modified)
                .Select(t => t.Entity);

        foreach (var entity in entities)
            entity.Update();
    }
}
