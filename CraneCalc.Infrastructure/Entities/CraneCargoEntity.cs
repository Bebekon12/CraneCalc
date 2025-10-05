using System.ComponentModel.DataAnnotations;

namespace CraneCalc.Infrastructure.Entities;

public class CraneCargoEntity
{
    [Key]
    public int Id { get; set; }
    public Guid CraneOrderId { get; set; }
        
    public Guid CargoId { get; set; }
        
    public string? SafetyComment { get; set; } = string.Empty;
    public double? CalculationResult { get; set; }
        
    public virtual CraneOrderEntity CraneOrder { get; set; } = null!;
        
    public virtual CargoEntity Cargo { get; set; } = null!;
}