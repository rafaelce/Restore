using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Infra.EntityFramework;

public class ApplicationContext : DbContext
{
    public required DbSet<Product> Products { get; set; }
    public required DbSet<Basket> Baskets { get; set; }


    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
