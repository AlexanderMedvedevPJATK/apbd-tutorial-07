using System.ComponentModel.DataAnnotations;

namespace Tutorial7.DTOs;

public class AddProductToWarehouseDto
{
    [Required]
    public int IdProduct { get; set; }
    
    [Required]
    public int IdWarehouse { get; set; }
    
    [Required]
    public int Amount { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    
}