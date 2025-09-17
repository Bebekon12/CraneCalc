namespace CraneCalc.Domain.Models;

public class CartCargoModel
{
    public int Id { get; set; }
    
    public Guid CartId { get; set; }
        
    public Guid CargoId { get; set; }
        
    public string SafetyComment { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
        
    public double CalculationResult { get; set; }
        
    public virtual CartModel CartModel { get; set; } = null!;
        
    public virtual CargoModel CargoModel { get; set; } = null!;
}