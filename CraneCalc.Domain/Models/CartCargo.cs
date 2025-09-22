namespace CraneCalc.Domain.Models;

public class CartCargo
{
    public int Id { get; set; }
    public Guid CartId { get; set; }
    public Guid CargoId { get; set; }
    public string SafetyComment { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public double CalculationResult { get; set; }
        
    public virtual Cart Cart { get; set; } = null!;
    public virtual Cargo Cargo { get; set; } = null!;
}