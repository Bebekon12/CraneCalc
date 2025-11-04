using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.UpdateCalculationResult;

public class UpdateCalculationResultCommandHandler(ICraneOrderRepository orderRepository)
    : IRequestHandler<UpdateCalculationResultCommand, bool>
{
    private const string ExpectedToken = "secret12345";

    public async Task<bool> Handle(UpdateCalculationResultCommand request, CancellationToken ct)
    {
        Console.WriteLine($"=== UPDATE CALCULATION ===");
        Console.WriteLine($"Received AuthToken: '{request.AuthToken}'");
        Console.WriteLine($"Expected Token: '{ExpectedToken}'");
        Console.WriteLine($"CraneOrderId: {request.CraneOrderId}");
        Console.WriteLine($"TotalCalculationResult: {request.TotalCalculationResult}");
        Console.WriteLine($"CargoResults count: {request.CargoResults.Count}");

        if (request.AuthToken != ExpectedToken)
        {
            Console.WriteLine("TOKEN VALIDATION FAILED!");
            return false;
        }

        Console.WriteLine("TOKEN VALIDATION SUCCESS!");

        try
        {
            await orderRepository.UpdateCalculationResultsAsync(
                Guid.Parse(request.CraneOrderId),
                request.TotalCalculationResult,
                request.CargoResults,
                ct
            );

            Console.WriteLine("Results updated successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating results: {ex.Message}");
            return false;
        }
    }
}