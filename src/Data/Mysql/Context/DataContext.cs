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

public class DataContext : IdentityDbContext<ApplicationUser>
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

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
            new IdentityRole { Name = "Moderator", NormalizedName = "MODERATOR" },
            new IdentityRole { Name = "User", NormalizedName = "USER" });
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
}
