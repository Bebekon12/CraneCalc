using CraneCalc.Application.Features.User.Dto;
using MediatR;

namespace CraneCalc.Application.Features.User.Queries.Me;

public record MeQuery : IRequest<UserResponse?>;