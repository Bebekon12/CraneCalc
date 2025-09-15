namespace CraneCalc.Infrastructure.Entities;

public class CartCargoEntity
{
    public int Id { get; set; }
    public Guid CartId { get; set; }
        
    public Guid CargoId { get; set; }
        
    public string SafetyComment { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;
        
    public double CalculationResult { get; set; }
        
    public virtual CartEntity Cart { get; set; } = null!;
        
    public virtual CargoEntity Cargo { get; set; } = null!;
}