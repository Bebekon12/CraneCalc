using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetFilteredCarts;

public record GetFilteredCartsQuery(DateTime From, DateTime Before) : IRequest<List<CartModel>>;