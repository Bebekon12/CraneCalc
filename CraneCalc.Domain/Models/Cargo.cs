namespace CraneCalc.Domain.Models;

public class Cargo
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public double Weight { get; init; }
    public Dimensions Dimensions { get; init; } = new();
    
    public bool IsDeleted { get; init; }
    
    public double Volume { get; init; }
    public string ConcreteGrade { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; } = string.Empty;
}

public class Dimensions
{
    public double Length { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }

    public override string ToString()
    {
        return $"{Length}м × {Width}м × {Height}м";
    }
}