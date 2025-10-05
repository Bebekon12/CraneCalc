namespace CraneCalc.Domain.Models;

public class CraneCargoModel
{
    public int Id { get; set; }
    
    public Guid CartId { get; set; }
        
    public Guid CargoId { get; set; }
        
    public string? SafetyComment { get; set; }
    
    public double? CalculationResult { get; set; }
        
    public virtual CraneOrderModel CraneOrderModel { get; set; } = null!;
        
    public virtual CargoModel CargoModel { get; set; } = null!;
}