using System.ComponentModel.DataAnnotations;

namespace DistribuidoraDonPepe.API.Models;

public class Customer
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string State { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
    
    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}