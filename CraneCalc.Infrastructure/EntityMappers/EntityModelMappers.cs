using AutoMapper;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;

namespace CraneCalc.Infrastructure.EntityMappers;

public class EntityModelMappers : Profile
{
    public EntityModelMappers()
    {
        CreateMap<CartModel, CartEntity>();
        CreateMap<CartEntity, CartModel>();
        
        CreateMap<CargoModel, CargoEntity>();
        CreateMap<CargoEntity, CargoModel>();
        
        CreateMap<CartCargoModel, CartCargoEntity>()
            .ForMember(dest=>dest.Cargo, opt=>opt.Ignore());
        CreateMap<CartCargoEntity, CartCargoModel>()
            .ForMember(dest=>dest.CargoModel, opt=>opt.Ignore());

        CreateMap<UserEntity, UserModel>();
        CreateMap<UserModel, UserEntity>();
    }
}