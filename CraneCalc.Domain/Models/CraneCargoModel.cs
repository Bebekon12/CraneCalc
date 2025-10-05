namespace CraneCalc.Domain.Models;

public class CraneCargoModel
{
    public int Id { get; set; }
    
    public Guid CraneOrderId { get; set; }
        
    public Guid CargoId { get; set; }
        
    public string? SafetyComment { get; set; }
    
    public double? CalculationResult { get; set; }
        
    public virtual CraneOrderModel CraneOrder { get; set; } = null!;
        
    public virtual CargoModel Cargo { get; set; } = null!;
}