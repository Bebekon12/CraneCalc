using AutoMapper;
using CraneCalc.Application.Contracts.Request;
using CraneCalc.Application.Contracts.Response;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.DtoModelMappers;

public class DtoModelMapper : Profile
{
    public DtoModelMapper()
    {
        CreateMap<Cargo, CreateCargoRequest>();
        CreateMap<CreateCargoRequest, Cargo>();
        
        CreateMap<Cargo, CargoResponse>();
        CreateMap<CargoResponse, Cargo>();
    }
}