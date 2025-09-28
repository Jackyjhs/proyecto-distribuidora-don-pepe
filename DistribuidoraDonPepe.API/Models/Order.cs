using System.ComponentModel.DataAnnotations;

namespace DistribuidoraDonPepe.API.Models;

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ShippedDate { get; set; }
    
    public DateTime? DeliveredDate { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    public decimal SubTotal { get; set; }
    
    public decimal Tax { get; set; }
    
    public decimal Total { get; set; }
    
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ShippingAddress { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}