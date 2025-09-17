using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.AddImageToCargo;

public record AddCargoImageCommand(
    Guid CargoId,
    Stream FileStream) : IRequest<string>;