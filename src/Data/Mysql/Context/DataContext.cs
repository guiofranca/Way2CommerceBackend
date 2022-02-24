using Domain.Models;
using Domain.Models.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mysql.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) { }
    public DataContext() { }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .AreUnicode(false)
            .HaveColumnType("varchar")
            .HaveMaxLength(255);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        var serverVersion = new MariaDbServerVersion(config["MariaDbServerVersion"]);
        optionsBuilder.UseMySql(config.GetConnectionString("DefaultConnection"), serverVersion);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder
        //     .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<ProductCategory>()
            .HasKey(pc => new { pc.ProductId, pc.CategoryId });
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
}
