using Domain.Identity;
using Domain.Identity.Relations;
using Domain.Models;
using Domain.Models.Relations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mysql.Identity;

namespace Mysql.Context;

public class DataContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
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
        if (optionsBuilder.IsConfigured) return;
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        var serverVersion = new MariaDbServerVersion(config["MariaDbServerVersion"]);
        optionsBuilder.UseMySql(config.GetConnectionString("DefaultConnection"), serverVersion);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasMany(c => c.Categories)
            .WithMany(c => c.Products)
            .UsingEntity<ProductCategory>();

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithMany(p => p.Categories)
            .UsingEntity<ProductCategory>();

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => p.DeletedAt == null);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(p => p.DeletedAt == null);

        modelBuilder.Entity<ApplicationRole>().HasData(
            new ApplicationRole { Id = new Guid("84f4de2d-45e1-44d6-ba6e-a99f2e9978d4"), Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
            new ApplicationRole { Id = new Guid("0bde5614-57c6-42ae-b8ca-d9310c7e5937"), Name = "Moderator", NormalizedName = "MODERATOR" },
            new ApplicationRole { Id = new Guid("1674307c-053c-4785-b567-87131195eae2"), Name = "User", NormalizedName = "USER" });
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
