using Microsoft.EntityFrameworkCore;
using DistribuidoraDonPepe.API.Models;

namespace DistribuidoraDonPepe.API.Data;

public class DistribuidoraDonPepeContext : DbContext
{
    public DistribuidoraDonPepeContext(DbContextOptions<DistribuidoraDonPepeContext> options) : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasIndex(e => e.Name);
        });
        
        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Name);
        });
        
        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.Tax).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Seed data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories and Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Arroz Premium",
                Description = "Arroz de alta calidad para distribución",
                Price = 2.50m,
                Stock = 100,
                Category = "Granos",
                Brand = "Don Pepe",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Frijoles Negros",
                Description = "Frijoles negros seleccionados",
                Price = 1.80m,
                Stock = 150,
                Category = "Granos",
                Brand = "Don Pepe",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 3,
                Name = "Aceite de Cocina",
                Description = "Aceite vegetal para cocinar",
                Price = 3.20m,
                Stock = 75,
                Category = "Aceites",
                Brand = "Don Pepe",
                CreatedDate = DateTime.UtcNow
            }
        );
        
        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                Name = "María García",
                Email = "maria.garcia@email.com",
                Phone = "555-0101",
                Address = "Calle Principal 123",
                City = "San José",
                State = "San José",
                PostalCode = "10101",
                CreatedDate = DateTime.UtcNow
            },
            new Customer
            {
                Id = 2,
                Name = "Juan Pérez",
                Email = "juan.perez@email.com",
                Phone = "555-0102",
                Address = "Avenida Central 456",
                City = "Cartago",
                State = "Cartago",
                PostalCode = "30101",
                CreatedDate = DateTime.UtcNow
            }
        );
    }
}