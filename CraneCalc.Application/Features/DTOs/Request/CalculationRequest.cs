using System.Text.Json.Serialization;

namespace CraneCalc.Application.Features.DTOs.Request;

public class CalculationRequest
{
    [JsonPropertyName("crane_order_id")]
    public string CraneOrderId { get; set; } 
    
    [JsonPropertyName("lifting_height")]
    public double? LiftingHeight { get; set; }
    
    [JsonPropertyName("lifting_speed")]
    public double? LiftingSpeed { get; set; }
    
    [JsonPropertyName("jib_outreach")]
    public double? JibOutreach { get; set; }
    
    [JsonPropertyName("cargos")]
    public List<CargoRequest> Cargos { get; set; } = new();
}

public class CargoRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }  // Изменяем на string для совместимости
    
    [JsonPropertyName("weight")]
    public double Weight { get; set; }
}