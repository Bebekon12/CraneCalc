using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.User.Queries.Me;

public record MeQuery : IRequest<UserModel?>;