using CraneCalc.Domain.Enums;

namespace CraneCalc.Domain.Models;

public class Cart
{
    public Guid Id { get; set; }
    public Status Status { get; set; } = Status.Draft;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int CreatorId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? FormationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int? ModeratorId { get; set; }
    public double LoadCapacity { get; set; }
    public double LiftingHeight { get; set; }
    public double JibOutreach { get; set; }
    public double LiftingSpeed { get; set; }
    public double CalculationResult { get; set; }
    public virtual List<CartCargo> CartCargo { get; set; } = [];
}