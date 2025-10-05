namespace CraneCalc.Infrastructure.Entities;

public class CraneCargoEntity
{
    public int Id { get; set; }
    public Guid CartId { get; set; }
        
    public Guid CargoId { get; set; }
        
    public string? SafetyComment { get; set; } = string.Empty;
    public double? CalculationResult { get; set; }
        
    public virtual CraneOrderEntity CraneOrder { get; set; } = null!;
        
    public virtual CargoEntity Cargo { get; set; } = null!;
}