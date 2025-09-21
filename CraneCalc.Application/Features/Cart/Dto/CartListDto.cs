using CraneCalc.Domain.Enums;

namespace CraneCalc.Application.Features.Cart.Dto;

public class CartListDto
{
    public Guid Id { get; set; }
        
    public Status Status { get; set; } = Status.Draft;
        
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string Creator { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;
        
    public DateTime? FormationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
        
    public string? Moderator { get; set; }
        
    public double LoadCapacity { get; set; }
    public double LiftingHeight { get; set; }
    public double JibOutreach { get; set; }
    public double LiftingSpeed { get; set; }
        
    public double CalculationResult { get; set; }
        
    public virtual List<Domain.Models.CartCargo> CartCargo { get; set; } = [];
}