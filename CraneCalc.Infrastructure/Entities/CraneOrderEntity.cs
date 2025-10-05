using CraneCalc.Domain.Enums;

namespace CraneCalc.Infrastructure.Entities;

public class CraneOrderEntity
{
    public Guid Id { get; set; }
    public Status Status { get; set; } = Status.Draft;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid CreatorId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? FormationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public Guid? ModeratorId { get; set; }
    public double? LoadCapacity { get; set; }
    public double? LiftingHeight { get; set; }
    public double? JibOutreach { get; set; }
    public double? LiftingSpeed { get; set; }
    public double? CalculationResult { get; set; }
    public virtual List<CraneCargoEntity> CartCargo { get; set; } = [];
}