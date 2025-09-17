using AutoMapper;
using CraneCalc.Application.Features.Cargo.Commands.CreateCargo;
using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.DtoModelMappers;

public class DtoModelMapper : Profile
{
    public DtoModelMapper()
    {
        CreateMap<Cargo, CreateCargoCommand>();
        CreateMap<CreateCargoCommand, Cargo>();
        
        CreateMap<Cargo, CargoResponse>();
        CreateMap<CargoResponse, Cargo>();
    }
}