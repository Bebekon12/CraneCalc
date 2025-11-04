using System.Net.Http.Json;
using System.Text.Json;
using CraneCalc.Application.Features.DTOs.Request;
using CraneCalc.Application.Features.DTOs.Response;
using CraneCalc.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CraneCalc.Application.Services;

public class CraneCalculationService : ICraneCalculationService
{
    private readonly HttpClient _httpClient;
    private const string FastApiBaseUrl = "http://localhost:8000";
    private readonly ILogger<CraneCalculationService> _logger;

    public CraneCalculationService(HttpClient httpClient, ILogger<CraneCalculationService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(FastApiBaseUrl);
        _logger = logger;
    }

    public async Task<CalculationResponse> CalculateCraneProductivityAsync(CalculationRequest request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Sending calculation request to FastAPI for order {OrderId}", 
                request.CraneOrderId);
            
            // Логируем данные запроса
            var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            _logger.LogInformation("Request data: {JsonRequest}", jsonRequest);

            var response = await _httpClient.PostAsJsonAsync("/calculate", request, ct);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CalculationResponse>(cancellationToken: ct);
                _logger.LogInformation("Calculation completed successfully for order {OrderId}", 
                    request.CraneOrderId);
                return result ?? new CalculationResponse { IsSuccess = false };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("FastAPI returned error {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
                
                return new CalculationResponse 
                { 
                    CraneOrderId = request.CraneOrderId,
                    IsSuccess = false 
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling FastAPI service for order {OrderId}", 
                request.CraneOrderId);
            
            return new CalculationResponse 
            { 
                CraneOrderId = request.CraneOrderId,
                IsSuccess = false 
            };
        }
    }
}