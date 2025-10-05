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
        CreateMap<CargoModel, CreateCargoCommand>();
        CreateMap<CreateCargoCommand, CargoModel>();
        
        CreateMap<CargoModel, CargoResponse>();
        CreateMap<CargoResponse, CargoModel>();
    }
}