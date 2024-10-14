using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{

    public DbSet<Product> Products { get; set; }

    public DataContext(DbContextOptions options) : base(options) {}
}
