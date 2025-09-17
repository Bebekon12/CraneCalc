namespace CraneCalc.Application.Contracts.Request;

public record CargoFilter
{
    public string? Title { get; set; }
    public string? Type { get; set; }
    public double? MinWeight { get; set; }
    public double? MaxWeight { get; set; }
}