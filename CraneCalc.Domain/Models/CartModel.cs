using CraneCalc.Domain.Enums;

namespace CraneCalc.Domain.Models;

public class CartModel
{
    public Guid Id { get; set; }
        
    public Status Status { get; set; } = Status.Draft;
        
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
    public Guid CreatorId { get; set; }
    
    public bool IsDeleted { get; set; } = false;
        
    public DateTime? FormationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
        
    public Guid? ModeratorId { get; set; }
        
    public double LoadCapacity { get; set; }
    public double LiftingHeight { get; set; }
    public double JibOutreach { get; set; }
    public double LiftingSpeed { get; set; }
        
    public double CalculationResult { get; set; }
        
    public virtual List<CartCargoModel> CartCargo { get; set; } = [];
}