using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.AddImageToCargo;

public class AddImageToCargoCommandHandler(ICargoRepository repository) : IRequestHandler<AddCargoImageCommand, string>
{
    public async Task<string> Handle(AddCargoImageCommand request, CancellationToken ct)
    {
        var file = await repository.AddOrUpdateCargoPhotoAsync(request.CargoId, request.FileStream, ct);
        
        return file;
    }
}