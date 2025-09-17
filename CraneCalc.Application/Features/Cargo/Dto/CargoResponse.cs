namespace CraneCalc.Application.Features.Cargo.Dto;

public record CargoResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public double Weight { get; init; }
    
    public double Length { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    
    public double Volume { get; init; }
    public string ConcreteGrade { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; } = string.Empty;
}