using System.ComponentModel.DataAnnotations;

namespace DistribuidoraDonPepe.API.Models;

public class OrderItem
{
    public int Id { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    public decimal Total => Quantity * UnitPrice;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}