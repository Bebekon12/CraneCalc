using AutoMapper;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;

namespace CraneCalc.Infrastructure.EntityMappers;

public class EntityModelMappers : Profile
{
    public EntityModelMappers()
    {
        CreateMap<CraneOrderModel, CraneOrderEntity>();
        CreateMap<CraneOrderEntity, CraneOrderModel>();
        
        CreateMap<CargoModel, CargoEntity>();
        CreateMap<CargoEntity, CargoModel>();
        
        CreateMap<CraneCargoModel, CraneCargoEntity>()
            .ForMember(dest=>dest.Cargo, opt=>opt.Ignore());
        CreateMap<CraneCargoEntity, CraneCargoModel>()
            .ForMember(dest=>dest.CargoModel, opt=>opt.Ignore());

        CreateMap<UserEntity, UserModel>();
        CreateMap<UserModel, UserEntity>();
    }
}